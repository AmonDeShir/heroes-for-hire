using System;
using System.Collections.Generic;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Buildings
{
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer))]
    public class BuildingCursor : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color successColor;
        [SerializeField] private Color errorColor;

        [Header("Follow")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float maxRayDistance = 500f;
        [SerializeField] private float yOffset = 0.02f;

        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleMask = ~0;

        private readonly List<GameObject> obstacles = new();

        private Material material;
        private Renderer cachedRenderer;
        private BoxCollider cachedCollider;

        private BuildingPlacementSelectionService selectionService;
        private BuildingCatalog buildingCatalog;
        private EventBinding<BuildingPlacementSelectedChangedEvent> buildingPlacementSelectedChangedEvent;

        [Inject]
        public void Construct(BuildingPlacementSelectionService selectionService, BuildingCatalog buildingCatalog)
        {
            this.selectionService = selectionService;
            this.buildingCatalog = buildingCatalog;
        }

        private void Awake()
        {
            cachedRenderer = GetComponent<Renderer>();
            cachedCollider = GetComponent<BoxCollider>();
            material = cachedRenderer.material;

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            buildingPlacementSelectedChangedEvent = new EventBinding<BuildingPlacementSelectedChangedEvent>(HandleSelectionChange);
            EventBus<BuildingPlacementSelectedChangedEvent>.Register(buildingPlacementSelectedChangedEvent);

            HandleSelectionChange(new BuildingPlacementSelectedChangedEvent { Value = selectionService.Selected });
            RefreshVisibility();
            UpdateColor();
        }

        private void OnDestroy()
        {
            EventBus<BuildingPlacementSelectedChangedEvent>.Unregister(buildingPlacementSelectedChangedEvent);
        }
        
        private void Update()
        {
            if (!HasSelection())
            {
                return;
            }

            FollowMouse();
        }

        private void HandleSelectionChange(BuildingPlacementSelectedChangedEvent @event)
        {
            var selected = @event.Value;
            var definition = buildingCatalog.GetById(selected);

            if (definition == null)
            {
                obstacles.Clear();
                ChangeSize(Vector3.zero);
                RefreshVisibility();
                UpdateColor();
                return;
            }

            ChangeSize(ReadBuildingSize(definition.Prefab.gameObject));
            RefreshVisibility();
            RecalculateObstacles();
        }

        private void FollowMouse()
        {
            if (worldCamera == null || Mouse.current == null)
            {
                return;
            }

            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, maxRayDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            var position = hit.point;
            position.y += yOffset;

            if ((transform.position - position).sqrMagnitude < 0.000001f)
            {
                return;
            }

            transform.position = position;
            RecalculateObstacles();
        }

        private void ChangeSize(Vector3 size)
        {
            transform.localScale = size;
        }

        private Vector3 ReadBuildingSize(GameObject building)
        {
            if (!building.TryGetComponent(out BoxCollider boxCollider))
            {
                return Vector3.zero;
            }

            return boxCollider.size;
        }

        private bool IsObstacle(GameObject obj)
        {
            return obj != gameObject && obj.TryGetComponent<IDamageable>(out _);
        }

        public bool HasObstacle()
        {
            return obstacles.Count > 0;
        }

        private bool HasSelection()
        {
            return !string.IsNullOrEmpty(selectionService.Selected);
        }

        private void RefreshVisibility()
        {
            var visible = HasSelection();
            cachedRenderer.enabled = visible;
            cachedCollider.enabled = visible;
        }

        private void UpdateColor()
        {
            material.color = HasObstacle() ? errorColor : successColor;
        }

        private void RecalculateObstacles()
        {
            obstacles.Clear();

            var bounds = cachedCollider.bounds;
            var hits = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                transform.rotation,
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );

            for (var i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];

                if (!IsObstacle(hit.gameObject))
                {
                    continue;
                }

                if (obstacles.Contains(hit.gameObject))
                {
                    continue;
                }

                obstacles.Add(hit.gameObject);
            }

            UpdateColor();
        }

        public Bounds GetCursorBounds()
        {
            return cachedCollider.bounds;
        }
    }
}
