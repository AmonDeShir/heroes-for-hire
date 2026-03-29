using Heroes.Game.Abstractions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

namespace Heroes.Presentation.World
{
    public class BuildingPlacementInput : MonoBehaviour
    {
        [SerializeField] private Camera placementCamera;
        [SerializeField] private LayerMask placementMask = ~0;
        [SerializeField] private float planeHeight;
        [SerializeField] private UIDocument uiDocument;

        private IBuildingPlacementService _placementService;
        private bool _overUi;

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

        private void OnEnable()
        {
            RegisterUiCallbacks();
        }

        private void OnDisable()
        {
            UnregisterUiCallbacks();
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

            if (_overUi)
            {
                return;
            }

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

        private void RegisterUiCallbacks()
        {
            var root = uiDocument != null ? uiDocument.rootVisualElement : null;

            if (root == null)
            {
                return;
            }

            root.RegisterCallback<PointerEnterEvent>(OnPointerEnterUi, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveUi, TrickleDown.TrickleDown);
        }

        private void UnregisterUiCallbacks()
        {
            var root = uiDocument != null ? uiDocument.rootVisualElement : null;

            if (root == null)
            {
                return;
            }

            root.UnregisterCallback<PointerEnterEvent>(OnPointerEnterUi, TrickleDown.TrickleDown);
            root.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveUi, TrickleDown.TrickleDown);
        }

        private void OnPointerEnterUi(PointerEnterEvent evt)
        {
            _overUi = true;
        }

        private void OnPointerLeaveUi(PointerLeaveEvent evt)
        {
            _overUi = false;
        }
    }
}
