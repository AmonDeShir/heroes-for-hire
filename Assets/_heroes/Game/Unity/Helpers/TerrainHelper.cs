using System.Collections.Generic;
using UnityEngine;

namespace Heroes
{
    [RequireComponent(typeof(Terrain))]
    [RequireComponent(typeof(TerrainCollider))]
    public class TerrainHelper : MonoBehaviour
    {
        [Header("Flatten")]
        [SerializeField] private float extraPadding = 1f;
        [SerializeField] private float blendBorder = 2f;
        [SerializeField] private bool useHighestPointInArea = true;

        [Header("Clear Trees")]
        [SerializeField] private bool removeTrees = true;
        [SerializeField] private bool removeDetails = true;
        [SerializeField] private float foliagePadding = 0.5f;

        private Terrain terrain;
        private TerrainData data;

        private void Awake()
        {
            terrain = GetComponent<Terrain>();

            data = Instantiate(terrain.terrainData);
            terrain.terrainData = data;

            var terrainCollider = GetComponent<TerrainCollider>();
            terrainCollider.terrainData = data;
        }

        public PreparedPlacement GetPreparedPlacement(Bounds buildingBounds, Vector3 currentPosition, float pivotToBottomOffset = 0f)
        {
            var flattenWorldY = GetTargetWorldHeight(buildingBounds, useHighestPointInArea);
            
            var buildingPosition = new Vector3(
                currentPosition.x,
                flattenWorldY + pivotToBottomOffset,
                currentPosition.z
            );

            return new PreparedPlacement(buildingPosition, flattenWorldY, buildingBounds);
        }
        
        public void PrepareAreaForBuilding(PreparedPlacement placement)
        {
            FlattenUnderBuilding(placement.Bounds, placement.FlattenWorldY);

            var foliageBounds = ExpandBoundsXZ(placement.Bounds, foliagePadding);

            if (removeTrees)
            {
                RemoveTrees(foliageBounds);
            }

            if (removeDetails)
            {
                RemoveDetails(foliageBounds);
            }
        }

        private void FlattenUnderBuilding(Bounds worldBounds, float targetWorldY)
        {
            var terrainPos = terrain.transform.position;
            var terrainSize = data.size;
            var resolution = data.heightmapResolution;

            var flattenBounds = ExpandBoundsXZ(worldBounds, extraPadding + blendBorder);
            var innerBounds = ExpandBoundsXZ(worldBounds, extraPadding);

            var area = GetHeightmapArea(flattenBounds, terrainPos, terrainSize, resolution);

            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            var heights = data.GetHeights(area.XMin, area.ZMin, area.Width, area.Height);
            var targetHeight = WorldHeightToNormalized(targetWorldY);

            for (var z = 0; z < area.Height; z++)
            {
                for (var x = 0; x < area.Width; x++)
                {
                    var worldPoint = GetHeightmapWorldPoint(
                        area.XMin + x,
                        area.ZMin + z,
                        terrainPos,
                        terrainSize,
                        resolution
                    );

                    var blend = GetBlend(
                        worldPoint.x,
                        worldPoint.z,
                        innerBounds.min.x,
                        innerBounds.max.x,
                        innerBounds.min.z,
                        innerBounds.max.z,
                        blendBorder
                    );

                    if (blend <= 0f)
                    {
                        continue;
                    }

                    heights[z, x] = Mathf.Lerp(heights[z, x], targetHeight, blend);
                }
            }

            data.SetHeights(area.XMin, area.ZMin, heights);
        }

        private void RemoveTrees(Bounds worldBounds)
        {
            var sourceTrees = data.treeInstances;
            if (sourceTrees == null || sourceTrees.Length == 0)
            {
                return;
            }

            data.treeInstances = GetTreesOutsideBounds(sourceTrees, worldBounds);
        }

        private TreeInstance[] GetTreesOutsideBounds(TreeInstance[] sourceTrees, Bounds worldBounds)
        {
            var result = new List<TreeInstance>(sourceTrees.Length);

            for (var i = 0; i < sourceTrees.Length; i++)
            {
                var worldPos = TreeToWorld(sourceTrees[i].position);

                if (!worldBounds.Contains(worldPos))
                {
                    result.Add(sourceTrees[i]);
                }
            }

            return result.ToArray();
        }

        private void RemoveDetails(Bounds worldBounds)
        {
            var area = GetDetailArea(worldBounds);
            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            var layerCount = data.detailPrototypes.Length;

            for (var layer = 0; layer < layerCount; layer++)
            {
                var details = data.GetDetailLayer(area.XMin, area.ZMin, area.Width, area.Height, layer);

                for (var z = 0; z < area.Height; z++)
                {
                    for (var x = 0; x < area.Width; x++)
                    {
                        details[z, x] = 0;
                    }
                }

                data.SetDetailLayer(area.XMin, area.ZMin, layer, details);
            }
        }

        private float GetTargetWorldHeight(Bounds bounds, bool useHighest)
        {
            var samplePoints = new[]
            {
                new Vector3(bounds.min.x, 0f, bounds.min.z),
                new Vector3(bounds.min.x, 0f, bounds.max.z),
                new Vector3(bounds.max.x, 0f, bounds.min.z),
                new Vector3(bounds.max.x, 0f, bounds.max.z),
                new Vector3(bounds.center.x, 0f, bounds.center.z),
            };

            if (useHighest)
            {
                var highest = float.MinValue;

                for (var i = 0; i < samplePoints.Length; i++)
                {
                    highest = Mathf.Max(highest, GetTerrainWorldHeight(samplePoints[i]));
                }

                return highest;
            }

            var sum = 0f;

            for (var i = 0; i < samplePoints.Length; i++)
            {
                sum += GetTerrainWorldHeight(samplePoints[i]);
            }

            return sum / samplePoints.Length;
        }

        private float GetTerrainWorldHeight(Vector3 worldPoint)
        {
            return terrain.SampleHeight(worldPoint) + terrain.transform.position.y;
        }

        private float WorldHeightToNormalized(float worldY)
        {
            return (worldY - terrain.transform.position.y) / data.size.y;
        }

        private Vector3 TreeToWorld(Vector3 normalizedTreePos)
        {
            return Vector3.Scale(normalizedTreePos, data.size) + terrain.transform.position;
        }

        private Vector3 GetHeightmapWorldPoint(int heightX, int heightZ, Vector3 terrainPos, Vector3 terrainSize, int resolution)
        {
            var worldX = terrainPos.x + (heightX / (float)(resolution - 1)) * terrainSize.x;
            var worldZ = terrainPos.z + (heightZ / (float)(resolution - 1)) * terrainSize.z;

            return new Vector3(worldX, 0f, worldZ);
        }

        private HeightmapArea GetHeightmapArea(Bounds worldBounds, Vector3 terrainPos, Vector3 terrainSize, int resolution)
        {
            var xMin = WorldToHeightX(worldBounds.min.x, terrainPos.x, terrainSize.x, resolution);
            var xMax = WorldToHeightX(worldBounds.max.x, terrainPos.x, terrainSize.x, resolution);
            var zMin = WorldToHeightZ(worldBounds.min.z, terrainPos.z, terrainSize.z, resolution);
            var zMax = WorldToHeightZ(worldBounds.max.z, terrainPos.z, terrainSize.z, resolution);

            return new HeightmapArea(xMin, xMax, zMin, zMax);
        }

        private DetailArea GetDetailArea(Bounds worldBounds)
        {
            var xMin = WorldToDetailX(worldBounds.min.x, data.detailWidth);
            var xMax = WorldToDetailX(worldBounds.max.x, data.detailWidth);
            var zMin = WorldToDetailZ(worldBounds.min.z, data.detailHeight);
            var zMax = WorldToDetailZ(worldBounds.max.z, data.detailHeight);

            return new DetailArea(xMin, xMax, zMin, zMax);
        }

        private Bounds ExpandBoundsXZ(Bounds bounds, float padding)
        {
            bounds.Expand(new Vector3(padding * 2f, 0f, padding * 2f));
            return bounds;
        }

        private int WorldToHeightX(float worldX, float terrainX, float terrainWidth, int resolution)
        {
            var normalized = Mathf.InverseLerp(terrainX, terrainX + terrainWidth, worldX);
            return Mathf.Clamp(Mathf.RoundToInt(normalized * (resolution - 1)), 0, resolution - 1);
        }

        private int WorldToHeightZ(float worldZ, float terrainZ, float terrainLength, int resolution)
        {
            var normalized = Mathf.InverseLerp(terrainZ, terrainZ + terrainLength, worldZ);
            return Mathf.Clamp(Mathf.RoundToInt(normalized * (resolution - 1)), 0, resolution - 1);
        }

        private int WorldToDetailX(float worldX, int resolution)
        {
            var terrainX = terrain.transform.position.x;
            var normalized = Mathf.InverseLerp(terrainX, terrainX + data.size.x, worldX);
            return Mathf.Clamp(Mathf.RoundToInt(normalized * (resolution - 1)), 0, resolution - 1);
        }

        private int WorldToDetailZ(float worldZ, int resolution)
        {
            var terrainZ = terrain.transform.position.z;
            var normalized = Mathf.InverseLerp(terrainZ, terrainZ + data.size.z, worldZ);
            return Mathf.Clamp(Mathf.RoundToInt(normalized * (resolution - 1)), 0, resolution - 1);
        }

        private static float GetBlend(float x, float z, float innerMinX, float innerMaxX, float innerMinZ, float innerMaxZ, float border)
        {
            if (IsInsideArea(x, z, innerMinX, innerMaxX, innerMinZ, innerMaxZ))
            {
                return 1f;
            }

            if (border <= 0.001f)
            {
                return 0f;
            }

            var distance = GetDistanceToArea(x, z, innerMinX, innerMaxX, innerMinZ, innerMaxZ);
            return Mathf.Clamp01(1f - distance / border);
        }

        private static bool IsInsideArea(float x, float z, float minX, float maxX, float minZ, float maxZ)
        {
            return x >= minX && x <= maxX && z >= minZ && z <= maxZ;
        }

        private static float GetDistanceToArea(float x, float z, float minX, float maxX, float minZ, float maxZ)
        {
            var dx = GetAxisDistanceToRange(x, minX, maxX);
            var dz = GetAxisDistanceToRange(z, minZ, maxZ);

            return Mathf.Max(dx, dz);
        }

        private static float GetAxisDistanceToRange(float value, float min, float max)
        {
            if (value < min)
            {
                return min - value;
            }

            if (value > max)
            {
                return value - max;
            }

            return 0f;
        }

        public readonly struct PreparedPlacement
        {
            public readonly Vector3 BuildingPosition;
            public readonly float FlattenWorldY;
            public readonly Bounds Bounds;

            public PreparedPlacement(Vector3 buildingPosition, float flattenWorldY, Bounds bounds)
            {
                BuildingPosition = buildingPosition;
                FlattenWorldY = flattenWorldY;
                Bounds = bounds;
            }
        }

        private readonly struct HeightmapArea
        {
            public readonly int XMin;
            public readonly int ZMin;
            public readonly int Width;
            public readonly int Height;

            public HeightmapArea(int xMin, int xMax, int zMin, int zMax)
            {
                XMin = xMin;
                ZMin = zMin;
                Width = xMax - xMin + 1;
                Height = zMax - zMin + 1;
            }
        }

        private readonly struct DetailArea
        {
            public readonly int XMin;
            public readonly int ZMin;
            public readonly int Width;
            public readonly int Height;

            public DetailArea(int xMin, int xMax, int zMin, int zMax)
            {
                XMin = xMin;
                ZMin = zMin;
                Width = xMax - xMin + 1;
                Height = zMax - zMin + 1;
            }
        }
    }
}

