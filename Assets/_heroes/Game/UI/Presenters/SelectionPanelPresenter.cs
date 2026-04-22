using System.Collections.Generic;
using System.Linq;
using Heroes.Content.Buildings;
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
        private SelectionService _selectionService;
        private BuildingUpgradeService _buildingUpgradeService;
        private EventBinding<ObjectSelectedEvent> _objectSelectedEvent;
        private EventBinding<HealthChangedEvent> _healthChangedEvent;
        private EventBinding<MaxHealthChangedEvent> _maxHealthChangedEvent;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyedEvent;
        private EventBinding<GoldChangedEvent> _goldChangedEvent;
        private EventBinding<UpgradeQueueChangedEvent> _upgradeQueueChangedEvent;
        private EventBinding<UpgradeQueueActiveChangedEvent> _upgradeQueueActiveChangedEvent;
        private EventBinding<UpgradeQueueProgressChangedEvent> _upgradeQueueProgressChangedEvent;
        private EventBinding<UpgradeQueueAvailableListChangedEvent> _upgradeQueueAvailableListChangedEvent;
        private EventBinding<UpgradeQueueUpgradeCompletedEvent> _upgradeQueueUpgradeCompletedEvent;

        [EventfulProperty] private SelectionDTO _selected;
        [EventfulProperty] private DamageableSelectionDTO _selectedDamageable;
        [EventfulProperty] private BuildingSelectionDTO _selectedBuilding;
        [EventfulProperty] private BuildingUpgradeSelectionDTO[] _buildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
        [EventfulProperty] private QueuedBuildingUpgradeSelectionDTO[] _queuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();

        [Inject]
        public void Construct(SelectionService selectionService, BuildingUpgradeService buildingUpgradeService)
        {
            _selectionService = selectionService;
            _buildingUpgradeService = buildingUpgradeService;

            _objectSelectedEvent = new EventBinding<ObjectSelectedEvent>(HandleSelectionChanged);
            _healthChangedEvent = new EventBinding<HealthChangedEvent>(HandleHealthSelectionChanged);
            _maxHealthChangedEvent = new EventBinding<MaxHealthChangedEvent>(HandleMaxHealthSelectionChanged);
            _buildingDestroyedEvent = new EventBinding<BuildingDestroyedEvent>(HandleBuildingDestroyed);
            _goldChangedEvent = new EventBinding<GoldChangedEvent>(_ => RefreshSelectedBuildingUpgrades());
            _upgradeQueueChangedEvent = new EventBinding<UpgradeQueueChangedEvent>(@event => HandleUpgradeQueueEvent(@event.Id));
            _upgradeQueueActiveChangedEvent = new EventBinding<UpgradeQueueActiveChangedEvent>(@event => HandleUpgradeQueueEvent(@event.Id));
            _upgradeQueueProgressChangedEvent = new EventBinding<UpgradeQueueProgressChangedEvent>(@event => HandleUpgradeQueueEvent(@event.Id));
            _upgradeQueueAvailableListChangedEvent = new EventBinding<UpgradeQueueAvailableListChangedEvent>(@event => HandleUpgradeQueueEvent(@event.Id));
            _upgradeQueueUpgradeCompletedEvent = new EventBinding<UpgradeQueueUpgradeCompletedEvent>(@event => HandleUpgradeQueueEvent(@event.Id));
            
            EventBus<ObjectSelectedEvent>.Register(_objectSelectedEvent);
            EventBus<HealthChangedEvent>.Register(_healthChangedEvent);
            EventBus<MaxHealthChangedEvent>.Register(_maxHealthChangedEvent);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyedEvent);
            EventBus<GoldChangedEvent>.Register(_goldChangedEvent);
            EventBus<UpgradeQueueChangedEvent>.Register(_upgradeQueueChangedEvent);
            EventBus<UpgradeQueueActiveChangedEvent>.Register(_upgradeQueueActiveChangedEvent);
            EventBus<UpgradeQueueProgressChangedEvent>.Register(_upgradeQueueProgressChangedEvent);
            EventBus<UpgradeQueueAvailableListChangedEvent>.Register(_upgradeQueueAvailableListChangedEvent);
            EventBus<UpgradeQueueUpgradeCompletedEvent>.Register(_upgradeQueueUpgradeCompletedEvent);

            if (selectionService.Selected != null)
            {
                HandleSelectionChanged(new ObjectSelectedEvent { Value = selectionService.Selected });
                RefreshSelectedBuildingUpgrades();
            }
        }

        public void SelectUpgrade(string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
            {
                return;
            }

            _buildingUpgradeService.TryQueueSelectedUpgrade(upgradeId);
        }
        
        private void HandleSelectionChanged(ObjectSelectedEvent obj)
        {
            if (obj.Value is null)
            {
                Selected = null;
                SelectedDamageable = null;
                SelectedBuilding = null;
                BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
                QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
                
                return;
            }

            if (obj.Value is ISelectable selectable)
            {
                Selected = new SelectionDTO(selectable.Id, selectable.Name, selectable.Description, selectable.Icon);
            }
            
            if (obj.Value is IDamageable damageable)
            {
                SelectedDamageable = new DamageableSelectionDTO(damageable.Health, damageable.MaxHealth);
            }
            
            if (obj.Value is BuildingFacade building)
            {
                SelectedBuilding = new BuildingSelectionDTO(building.IsAlive);
                RefreshSelectedBuildingUpgrades();
                return;
            }

            SelectedBuilding = null;
            BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
            QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
        }

        private void HandleHealthSelectionChanged(HealthChangedEvent obj)
        {
            if (Selected?.Id != obj.Id)
            {
                return;
            }

            SelectedDamageable = new DamageableSelectionDTO(obj.Value, SelectedDamageable.MaxHealth);
        }

        private void HandleMaxHealthSelectionChanged(MaxHealthChangedEvent obj)
        {
            if (Selected?.Id != obj.Id || SelectedDamageable == null)
            {
                return;
            }

            SelectedDamageable = new DamageableSelectionDTO(SelectedDamageable.CurrentHealth, obj.Value);
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
            BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
            QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
        }

        private void HandleUpgradeQueueEvent(string buildingId)
        {
            if (Selected?.Id != buildingId)
            {
                return;
            }

            RefreshSelectedBuildingUpgrades();
        }

        private void RefreshSelectedBuildingUpgrades()
        {
            if (_selectionService.Selected is not BuildingFacade building || building.Definition == null || building.Model == null)
            {
                BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
                QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
                return;
            }

            var items = new List<BuildingUpgradeSelectionDTO>();
            var queuedIds = building.Model.UpgradeQueue.Queue.ToArray();
            var queuedItems = new List<QueuedBuildingUpgradeSelectionDTO>();

            for (var i = 0; i < queuedIds.Length; i++)
            {
                var queuedUpgrade = GetUpgradeDefinition(building.Definition, queuedIds[i]);
                if (queuedUpgrade == null)
                {
                    continue;
                }

                queuedItems.Add(new QueuedBuildingUpgradeSelectionDTO(
                    queuedUpgrade.Id,
                    queuedUpgrade.Name,
                    queuedUpgrade.Description,
                    queuedUpgrade.IconPath,
                    i));
            }

            foreach (var upgrade in EnumerateUpgrades(building.Definition.AvailableUpgrades))
            {
                var completedCount = building.Model.UpgradeQueue.GetCompletedCount(upgrade.Id);
                var isCompleted = upgrade.UsageLimit > 0 && completedCount >= upgrade.UsageLimit;
                var isActive = building.Model.UpgradeQueue.active == upgrade.Id;
                var isQueued = isActive || building.Model.UpgradeQueue.Queue.Contains(upgrade.Id);
                var progress = isActive ? building.Model.UpgradeQueue.progress : 0f;
                var queueIndex = System.Array.IndexOf(queuedIds, upgrade.Id);

                items.Add(new BuildingUpgradeSelectionDTO(
                    upgrade.Id,
                    upgrade.Name,
                    upgrade.Description,
                    (int)upgrade.GoldCost,
                    upgrade.IconPath,
                    isQueued,
                    isActive,
                    isCompleted,
                    _buildingUpgradeService.CanQueueUpgrade(building, upgrade.Id),
                    _buildingUpgradeService.GetQueueLockReason(building, upgrade.Id),
                    progress,
                    queueIndex));
            }

            BuildingUpgrades = items.ToArray();
            QueuedBuildingUpgrades = queuedItems.ToArray();
        }

        private static BuildingUpgradeDefinition GetUpgradeDefinition(BuildingDefinition definition, string upgradeId)
        {
            if (definition == null || string.IsNullOrWhiteSpace(upgradeId))
            {
                return null;
            }

            return EnumerateUpgrades(definition.AvailableUpgrades).FirstOrDefault(upgrade => upgrade.Id == upgradeId);
        }

        private static IEnumerable<BuildingUpgradeDefinition> EnumerateUpgrades(IEnumerable<BuildingUpgradeDefinition> upgrades)
        {
            if (upgrades == null)
            {
                yield break;
            }

            var visited = new HashSet<string>();
            var stack = new Stack<BuildingUpgradeDefinition>(upgrades.Where(item => item != null));

            while (stack.Count > 0)
            {
                var upgrade = stack.Pop();
                if (upgrade == null || string.IsNullOrWhiteSpace(upgrade.Id) || !visited.Add(upgrade.Id))
                {
                    continue;
                }

                yield return upgrade;

                if (upgrade.UnlocksUpgrades == null)
                {
                    continue;
                }

                foreach (var unlockedUpgrade in upgrade.UnlocksUpgrades)
                {
                    if (unlockedUpgrade != null)
                    {
                        stack.Push(unlockedUpgrade);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            EventBus<ObjectSelectedEvent>.Unregister(_objectSelectedEvent);
            EventBus<HealthChangedEvent>.Unregister(_healthChangedEvent);
            EventBus<MaxHealthChangedEvent>.Unregister(_maxHealthChangedEvent);
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyedEvent);
            EventBus<GoldChangedEvent>.Unregister(_goldChangedEvent);
            EventBus<UpgradeQueueChangedEvent>.Unregister(_upgradeQueueChangedEvent);
            EventBus<UpgradeQueueActiveChangedEvent>.Unregister(_upgradeQueueActiveChangedEvent);
            EventBus<UpgradeQueueProgressChangedEvent>.Unregister(_upgradeQueueProgressChangedEvent);
            EventBus<UpgradeQueueAvailableListChangedEvent>.Unregister(_upgradeQueueAvailableListChangedEvent);
            EventBus<UpgradeQueueUpgradeCompletedEvent>.Unregister(_upgradeQueueUpgradeCompletedEvent);
        }
    }
}
