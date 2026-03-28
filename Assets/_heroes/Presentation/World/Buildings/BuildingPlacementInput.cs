using Heroes.Game.Abstractions;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Presentation.World
{
    public class BuildingPlacementInput : MonoBehaviour
    {
        [SerializeField] private Camera placementCamera;
        [SerializeField] private LayerMask placementMask = ~0;
        [SerializeField] private float planeHeight;

        private IBuildingPlacementService _placementService;

        [Inject]
        public void Construct(IBuildingPlacementService placementService)
        {
            _placementService = placementService;
        }

        private void Awake()
        {
            if (placementCamera == null)
            {
                placementCamera = Camera.main;
            }
        }

        private void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame)
            {
                return;
            }

            if (placementCamera == null)
            {
                return;
            }

            var screenPosition = pointer.position.ReadValue();
            if (!TryGetPlacementPoint(screenPosition, out var worldPosition))
            {
                return;
            }

            _placementService.TryPlaceSelectedBuilding(worldPosition);
        }

        private bool TryGetPlacementPoint(Vector2 screenPosition, out Vector2 worldPosition)
        {
            var ray = placementCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, placementMask))
            {
                worldPosition = new Vector2(hit.point.x, hit.point.z);
                return true;
            }

            var plane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));
            if (plane.Raycast(ray, out var distance))
            {
                var point = ray.GetPoint(distance);
                worldPosition = new Vector2(point.x, point.z);
                return true;
            }

            worldPosition = default;
            return false;
        }
    }
}
