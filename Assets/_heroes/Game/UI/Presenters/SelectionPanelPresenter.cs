using System.Collections.Generic;
using System.Linq;
using Heroes.Content.Buildings;
using EventBus;
using Heroes.Game.Abstractions;
using Heroes.Game.AI;
using Heroes.Game.Heroes;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Content.Heroes;
using Heroes.GOAP.Core.Debug;
using Heroes.Presentation.UI.BuildingPanel;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.SelectionPanel
{
    public partial class SelectionPanelPresenter : MonoBehaviour
    {
        private SelectionService _selectionService;
        private BuildingUpgradeService _buildingUpgradeService;
        private ItemCatalog _itemCatalog;
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
        private float _nextHeroRefreshTime;

        [EventfulProperty] private SelectionDTO _selected;
        [EventfulProperty] private DamageableSelectionDTO _selectedDamageable;
        [EventfulProperty] private BuildingSelectionDTO _selectedBuilding;
        [EventfulProperty] private HeroSelectionDTO _selectedHero;
        [EventfulProperty] private HeroEquipmentSelectionDTO _selectedHeroEquipment;
        [EventfulProperty] private GoapSelectionDTO _selectedGoap;
        [EventfulProperty] private ShopItemSelectionDTO[] _shopItems = System.Array.Empty<ShopItemSelectionDTO>();
        [EventfulProperty] private BuildingUpgradeSelectionDTO[] _buildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
        [EventfulProperty] private QueuedBuildingUpgradeSelectionDTO[] _queuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();

        [Inject]
        public void Construct(SelectionService selectionService, BuildingUpgradeService buildingUpgradeService, ItemCatalog itemCatalog)
        {
            _selectionService = selectionService;
            _buildingUpgradeService = buildingUpgradeService;
            _itemCatalog = itemCatalog;

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

        private void RefreshSelectedBuilding()
        {
            if (_selectionService.Selected is not BuildingFacade building)
            {
                SelectedBuilding = null;
                return;
            }

            SelectedBuilding = new BuildingSelectionDTO(building.IsAlive);
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
                SelectedHero = null;
                SelectedHeroEquipment = null;
                SelectedGoap = null;
                ShopItems = System.Array.Empty<ShopItemSelectionDTO>();
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
                SelectedHero = null;
                SelectedHeroEquipment = null;
                SelectedGoap = null;
                RefreshSelectedShopItems();
                RefreshSelectedBuildingUpgrades();
                return;
            }

            if (obj.Value is HeroFacade hero)
            {
                RefreshSelectedHero(hero);
                return;
            }

            SelectedBuilding = null;
            SelectedHero = null;
            SelectedHeroEquipment = null;
            SelectedGoap = null;
            ShopItems = System.Array.Empty<ShopItemSelectionDTO>();
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
            SelectedHero = null;
            SelectedGoap = null;
            BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
            QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextHeroRefreshTime)
            {
                return;
            }

            _nextHeroRefreshTime = Time.unscaledTime + 0.2f;

            if (_selectionService?.Selected is HeroFacade hero)
            {
                RefreshSelectedHero(hero);
            }
        }

        private void RefreshSelectedHero(HeroFacade hero)
        {
            SelectedBuilding = null;
            ShopItems = System.Array.Empty<ShopItemSelectionDTO>();
            BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
            QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();

            if (hero?.Model == null)
            {
                SelectedHero = null;
                SelectedHeroEquipment = null;
                SelectedGoap = null;
                return;
            }

            var baseAttack = hero.Definition != null ? hero.Definition.Attack : 0f;
            var baseDefence = hero.Definition != null ? hero.Definition.Defence : 0f;
            var baseSpeed = hero.Definition != null ? hero.Definition.Speed : 0f;

            var eqAttack = hero.Model.EquipmentAttack;
            var eqDefence = hero.Model.EquipmentDefence;
            var eqSpeed = hero.Model.EquipmentSpeed;

            var timedAttack = hero.Model.TimedAttack;
            var timedDefence = hero.Model.TimedDefence;
            var timedSpeed = hero.Model.TimedSpeed;

            SelectedHero = new HeroSelectionDTO(
                hero.Model.Gold,
                hero.Model.GearLevel,
                hero.Model.DangerLevel,
                hero.Model.IsAlive,
                hero.Model.IsInHome,
                baseAttack + eqAttack + timedAttack,
                baseDefence + eqDefence + timedDefence,
                baseSpeed + eqSpeed + timedSpeed);

            SelectedHeroEquipment = new HeroEquipmentSelectionDTO(
                ResolveItem(hero.Model.EquippedWeaponId),
                ResolveItem(hero.Model.EquippedArmorId),
                ResolveItems(hero.Model.EquippedArtifacts),
                ResolveItems(hero.Model.EquippedConsumables),
                ResolveItems(hero.Model.Backpack));

            if (hero.TryGetComponent<HeroAgent>(out var heroAgent) && heroAgent.TryGetSnapshot(out var snapshot))
            {
                SelectedGoap = ToGoapSelection(snapshot);
            }
            else
            {
                SelectedGoap = null;
            }
        }

        private static GoapSelectionDTO ToGoapSelection(GoapDebugSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            
            var goals = snapshot.Goals
                    .Where(g => g != null && g.Importance > 0.001f)
                    .OrderByDescending(item => item.Priority)
                    .Select(item => new GoapGoalSelectionDTO(item.Name, item.Description, item.Icon, item.Heuristic, item.Name == snapshot?.Plan.GoalName))
                    .ToArray()
                ?? System.Array.Empty<GoapGoalSelectionDTO>();
            var steps = snapshot.Plan?.Steps?.Select(item => new GoapPlanStepSelectionDTO(item.Name, item.Description, item.Icon, item.PreconditionsMet)).ToArray()
                ?? System.Array.Empty<GoapPlanStepSelectionDTO>();

            return new GoapSelectionDTO(goals, steps);
        }

        private void HandleUpgradeQueueEvent(string buildingId)
        {
            if (Selected?.Id != buildingId)
            {
                return;
            }

            RefreshSelectedShopItems();
            RefreshSelectedBuildingUpgrades();
        }

        private EquipmentItemDTO ResolveItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId) || _itemCatalog == null)
            {
                return null;
            }
            
            var def = _itemCatalog.GetById(itemId);

            if (def == null)
            {
                return null;
            }
            
            return new EquipmentItemDTO
            {
                Icon = def.IconPath,
                Name = def.DisplayName
            };
        }

        private EquipmentItemDTO[] ResolveItems(IEnumerable<string> itemIds)
        {
            if (itemIds == null)
            {
                return System.Array.Empty<EquipmentItemDTO>();
            }

            var list = new List<EquipmentItemDTO>();
            
            foreach (var id in itemIds)
            {
                var name = ResolveItem(id);
                
                if (name != null)
                {
                    list.Add(name);
                }
            }

            return list.ToArray();
        }

        private void RefreshSelectedShopItems()
        {
            if (_selectionService.Selected is not BuildingFacade building || building.Definition == null || building.Model == null)
            {
                ShopItems = System.Array.Empty<ShopItemSelectionDTO>();
                return;
            }

            var sellItems = building.Definition.SellItems;
            if (sellItems == null || sellItems.Length == 0)
            {
                ShopItems = System.Array.Empty<ShopItemSelectionDTO>();
                return;
            }

            var items = new List<ShopItemSelectionDTO>(sellItems.Length);
            foreach (var item in sellItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id))
                {
                    continue;
                }

                var unlocked = building.Model.IsSellItemUnlocked(item.Id);
                items.Add(new ShopItemSelectionDTO(
                    item.Id,
                    item.DisplayName,
                    item.Description,
                    item.IconPath,
                    item.GoldCost,
                    item.Attack,
                    item.Defense,
                    item.Speed,
                    item.HpRegeneration,
                    item.Slot.ToString(),
                    item.IsSingleUse,
                    unlocked,
                    unlocked ? null : "Requires research"));
            }

            ShopItems = items.ToArray();
        }

        private void RefreshSelectedBuildingUpgrades()
        {
            if (_selectionService.Selected is not BuildingFacade building || building.Definition == null || building.Model == null)
            {
                BuildingUpgrades = System.Array.Empty<BuildingUpgradeSelectionDTO>();
                QueuedBuildingUpgrades = System.Array.Empty<QueuedBuildingUpgradeSelectionDTO>();
                return;
            }

            RefreshSelectedBuilding();

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


