using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public class BuildingPlacementController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LayerMask groundMask;
        public BuildingPlacementService PlacementService { get; private set; }
        public BuildingPlacementSelectionService SelectionService { get; private set; }
        private BuildingCatalog _buildingCatalog;

        [Inject]
        public void Construct(BuildingPlacementService placementService, BuildingPlacementSelectionService selectionService, BuildingCatalog buildingCatalog)
        {
            PlacementService = placementService;
            SelectionService = selectionService;
            _buildingCatalog = buildingCatalog;
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(SelectionService.Selected))
            {
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, 500f, groundMask))
            {
                return;
            }

            var definition = _buildingCatalog.GetById(SelectionService.Selected);

            if (PlacementService.TryPlace(definition, hit.point, Quaternion.identity, out _))
            {
                SelectionService.Clear();
            }
        }
    }
}
