using System;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Registry;
using UnityEngine;
using Heroes;
using Heroes.Game.Combat;

namespace Heroes.Game.Monsters
{
    public sealed class MonsterLairBootstrap : MonoBehaviour
    {
        private void Start()
        {
            var points = UnityEngine.Object.FindObjectsByType<MonsterLairSpawnPoint>(FindObjectsSortMode.None);
            for (var i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (p == null || !p.SpawnOnStart)
                {
                    continue;
                }

                SpawnOne(p);
            }
        }

        private static void SpawnOne(MonsterLairSpawnPoint point)
        {
            var def = point.BuildingDefinition;
            if (def == null || def.Prefab == null)
            {
                return;
            }

            var pos = point.transform.position;
            var rot = point.transform.rotation;
            var go = UnityEngine.Object.Instantiate(def.Prefab, pos, rot);

            var faction = go.GetComponent<Faction>();
            if (faction == null)
            {
                faction = go.gameObject.AddComponent<Faction>();
            }
            faction.Team = TeamType.Enemies;

            var terrain = TerrainHelper.FindForPosition(pos);
            if (terrain != null)
            {
                var col = go.GetComponentInChildren<Collider>();
                if (col != null)
                {
                    var placement = terrain.GetPreparedPlacement(col.bounds, pos);
                    terrain.PrepareAreaForBuilding(placement);
                    go.transform.position = placement.BuildingPosition;
                }
            }

            var instanceId = Guid.NewGuid().ToString();
            go.Initialize(def, instanceId);
            Registry<BuildingFacade>.TryAdd(go);

            var spawner = go.GetComponent<MonsterLairSpawner>();
            if (spawner == null)
            {
                spawner = go.gameObject.AddComponent<MonsterLairSpawner>();
            }

            spawner.Configure(point.LairDefinition, point.MonsterSpawnPoint);

            EventBus<BuildingPlacedEvent>.Invoke(new BuildingPlacedEvent
            {
                InstanceId = instanceId,
                DefinitionId = def.Id,
                Position = go.transform.position,
            });
        }
    }
}
