using Heroes.Content.Buildings;
using Heroes;
using Heroes.Game.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public class BuildingPlacementController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private BuildingCursor buildingCursor;

        [Header("FX")]
        [SerializeField] private ParticleSystem placementBurstPrefab;
        [SerializeField] private AudioSource placementAudio;
        
        public BuildingPlacementService PlacementService { get; private set; }
        public BuildingPlacementSelectionService SelectionService { get; private set; }
        private BuildingCatalog _buildingCatalog;

        private bool inBuildingState = false;

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
            _ = HandlePlacement();
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
            if (inBuildingState)
            {
                return;   
            }

            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (IsPointerOverUi())
            {
                return;
            }

            if (buildingCursor.HasObstacle())
            {
                return;
            }
            
            var definition = _buildingCatalog.GetById(SelectionService.Selected);
            
            if (definition == null)
            {
                return;
            }

            if (!PlacementService.CanBuild(definition))
            {
                return;
            }

            var helper = TerrainHelper.FindForPosition(buildingCursor.transform.position);
            if (helper == null)
            {
                return;
            }

            var placement = helper.GetPreparedPlacement(buildingCursor.GetCursorBounds(), buildingCursor.transform.position);
            await RunPlacementSequence(definition, placement);
        }

        private static bool IsPointerOverUi()
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current == null)
                {
                    return false;
                }
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }

            return UiInputGate.CursorOnBlockingUi;
        }

        private async Awaitable RunPlacementSequence(BuildingDefinition definition, TerrainHelper.PreparedPlacement placement)
        {
            inBuildingState = true;
            SelectionService.Clear();
            buildingCursor.gameObject.SetActive(false);

            try
            {
                PlayPlacementEffects(placement.BuildingPosition);
                await Awaitable.WaitForSecondsAsync(0.5f);
                var helper = TerrainHelper.FindForPosition(placement.BuildingPosition);
                helper?.PrepareAreaForBuilding(placement);
                PlacementService.TryPlace(definition, placement.BuildingPosition, Quaternion.identity, out _);
            }
            finally
            {
                inBuildingState = false;
                buildingCursor.gameObject.SetActive(true);
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


