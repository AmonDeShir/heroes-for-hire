using System.Threading.Tasks;
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
        [SerializeField] private TerrainHelper terrainHelper;
        [SerializeField] private BuildingCursor buildingCursor;

        [Header("FX")]
        [SerializeField] private ParticleSystem placementBurstPrefab;
        [SerializeField] private AudioSource placementAudio;
        
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

            HandleCancel();
            HandlePlacement();
        }

        private void HandleCancel()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SelectionService.Clear();
                return;
            }

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                SelectionService.Clear();
            }
        }

        private async Awaitable HandlePlacement()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (buildingCursor.HasObstacle())
            {
                return;
            }

            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, 500f, groundMask))
            {
                return;
            }
            
            var definition = _buildingCatalog.GetById(SelectionService.Selected);
            
            if (definition == null)
            {
                return;
            }

            var placement = terrainHelper.GetPreparedPlacement(buildingCursor.GetCursorBounds(), buildingCursor.transform.position);

            if (PlacementService.CanBuild(definition))
            {
                SelectionService.Clear();
                
                PlayPlacementEffects(placement.BuildingPosition);
                await Awaitable.WaitForSecondsAsync(0.5f);
                terrainHelper.PrepareAreaForBuilding(placement);
                
                PlacementService.TryPlace(definition, placement.BuildingPosition, Quaternion.identity, out _);
            }
        }
        
        private void PlayPlacementEffects(Vector3 position)
        {
            if (placementBurstPrefab != null)
            {
                var particles = Instantiate(placementBurstPrefab, position, Quaternion.identity);
                particles.Play();
                Destroy(particles.gameObject, 6f);
            }

            if (placementAudio)
            {
                placementAudio.time = 0f;
                placementAudio.Play();
            }
        }
    }
}   