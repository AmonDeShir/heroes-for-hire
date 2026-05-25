using System;
using System.Collections.Generic;
using EventBus;
using Heroes;
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
        [SerializeField] private float maxRayDistance = 500f;
        [SerializeField] private float yOffset = 0.02f;

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

            if (cachedCollider != null)
            {
                cachedCollider.isTrigger = true;
            }

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
            obstacles.Clear();
            UpdateColor();
        }

        private void FollowMouse()
        {
            if (worldCamera == null || Mouse.current == null)
            {
                return;
            }

            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Mathf.Abs(ray.direction.y) < 0.0001f)
            {
                return;
            }

            var t = -ray.origin.y / ray.direction.y;
            if (t <= 0f || t > maxRayDistance)
            {
                return;
            }

            var position = ray.origin + ray.direction * t;
            var helper = TerrainHelper.FindForPosition(position);
            if (helper != null)
            {
                position.y = helper.GetWorldHeight(position) + yOffset;
            }
            else
            {
                position.y = yOffset;
            }

            if ((transform.position - position).sqrMagnitude < 0.000001f)
            {
                return;
            }

            transform.position = position;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
            {
                return;
            }

            var obj = other.gameObject;
            if (obj == null || !IsObstacle(obj))
            {
                return;
            }

            if (!obstacles.Contains(obj))
            {
                obstacles.Add(obj);
                UpdateColor();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == null)
            {
                return;
            }

            var obj = other.gameObject;
            if (obj == null)
            {
                return;
            }

            if (obstacles.Remove(obj))
            {
                UpdateColor();
            }
        }

        public Bounds GetCursorBounds()
        {
            return cachedCollider.bounds;
        }
    }
}


