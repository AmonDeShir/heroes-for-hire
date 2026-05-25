using System.Linq;
using EventBus;
using Heroes.Game.Combat;
using Heroes.Game.Core.Events;
using Heroes.Game.Buildings;
using Heroes.Game.AI;
using OneJS;
using Registry;
using UnityEngine;

namespace Heroes.Presentation.UI.GameEnd
{
    public partial class GameEndPresenter : MonoBehaviour
    {
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyed;

        [EventfulProperty] private bool _isOpen;
        [EventfulProperty] private string _message;

        private void Awake()
        {
            _buildingDestroyed = new EventBinding<BuildingDestroyedEvent>(OnBuildingDestroyed);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyed);
        }

        private void OnDestroy()
        {
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyed);
        }

        private void OnBuildingDestroyed(BuildingDestroyedEvent e)
        {
            if (IsOpen)
            {
                return;
            }

            var castleDefId = GoapRuntimeConfig.Buildings != null && GoapRuntimeConfig.Buildings.Castle != null
                ? GoapRuntimeConfig.Buildings.Castle.Id
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(castleDefId) && e.DefinitionId == castleDefId)
            {
                Message = "Defeat";
                IsOpen = true;
                return;
            }

            var anyEnemy = Registry<BuildingFacade>.All().Any(b =>
                b != null && b.IsAlive && b.TryGetComponent<Faction>(out var f) && f != null && f.Team == TeamType.Enemies);

            if (!anyEnemy)
            {
                Message = "Victory";
                IsOpen = true;
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
