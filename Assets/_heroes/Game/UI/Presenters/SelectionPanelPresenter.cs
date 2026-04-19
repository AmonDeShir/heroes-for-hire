using EventBus;
using Heroes.Game.Abstractions;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.SelectionPanel
{
    public partial class SelectionPanelPresenter : MonoBehaviour
    {
        private EventBinding<ObjectSelectedEvent> _objectSelectedEvent;
        private EventBinding<HealthChangedEvent> _healthChangedEvent;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyedEvent;

        [EventfulProperty] private SelectionDTO _selected;
        [EventfulProperty] private DamageableSelectionDTO _selectedDamageable;
        [EventfulProperty] private BuildingSelectionDTO _selectedBuilding;

        [Inject]
        public void Construct(SelectionService selectionService)
        {
            _objectSelectedEvent = new EventBinding<ObjectSelectedEvent>(HandleSelectionChanged);
            _healthChangedEvent = new EventBinding<HealthChangedEvent>(HandleHealthSelectionChanged);
            _buildingDestroyedEvent = new EventBinding<BuildingDestroyedEvent>(HandleBuildingDestroyed);
            
            EventBus<ObjectSelectedEvent>.Register(_objectSelectedEvent);
            EventBus<HealthChangedEvent>.Register(_healthChangedEvent);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyedEvent);

            if (selectionService.Selected != null)
            {
                HandleSelectionChanged(new ObjectSelectedEvent { value = selectionService.Selected });
            }
        }
        
        private void HandleSelectionChanged(ObjectSelectedEvent obj)
        {
            if (obj.value is null)
            {
                Selected = null;
                SelectedDamageable = null;
                SelectedBuilding = null;
                
                return;
            }

            if (obj.value is ISelectable selectable)
            {
                Selected = new SelectionDTO(selectable.Id, selectable.Name, selectable.Description, selectable.Icon);
            }
            
            if (obj.value is IDamageable damageable)
            {
                SelectedDamageable = new DamageableSelectionDTO(damageable.Health, damageable.MaxHealth);
            }
            
            if (obj.value is BuildingFacade building)
            {
                SelectedBuilding = new BuildingSelectionDTO(building.IsAlive);
            }
        }

        private void HandleHealthSelectionChanged(HealthChangedEvent obj)
        {
            if (Selected?.Id != obj.Id)
            {
                return;
            }

            SelectedDamageable = new DamageableSelectionDTO(obj.NewValue, SelectedDamageable.MaxHealth);
        }
        
        private void HandleBuildingDestroyed(BuildingDestroyedEvent obj)
        {
            if (Selected?.Id != obj.InstanceId)
            {
                return;
            }

            Selected = null;
            SelectedDamageable = null;
            SelectedBuilding = null;
        }

        private void OnDestroy()
        {
            EventBus<ObjectSelectedEvent>.Unregister(_objectSelectedEvent);
            EventBus<HealthChangedEvent>.Unregister(_healthChangedEvent);
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyedEvent);        
        }
    }
}
