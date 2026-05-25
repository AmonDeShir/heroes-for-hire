using System;
using Heroes.Presentation.UI.BuildingPanel;

namespace Heroes.Presentation.UI.SelectionPanel
{
    public class SelectionDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public string Icon;
        
        public SelectionDTO(string id, string name, string description, string icon)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
        }
    }

    public class DamageableSelectionDTO
    {
        public float CurrentHealth;
        public float MaxHealth;

        public DamageableSelectionDTO(float current, float max)
        {
            CurrentHealth = current;
            MaxHealth = max;
        }
    }

    public class BuildingSelectionDTO
    {
        public bool IsAlive;
        public bool IsChapel;

        public BuildingSelectionDTO(bool isAlive, bool isChapel)
        {
            IsAlive = isAlive;
            IsChapel = isChapel;
        }
    }

    public class HeroSelectionDTO
    {
        public int Gold;
        public float GearLevel;
        public float DangerLevel;
        public bool IsAlive;
        public bool IsInHome;

        public float Attack;
        public float Defence;
        public float Speed;

        public HeroSelectionDTO(int gold, float gearLevel, float dangerLevel, bool isAlive, bool isInHome, float attack, float defence, float speed)
        {
            Gold = gold;
            GearLevel = gearLevel;
            DangerLevel = dangerLevel;
            IsAlive = isAlive;
            IsInHome = isInHome;

            Attack = attack;
            Defence = defence;
            Speed = speed;
        }
    }

    public class HeroEquipmentSelectionDTO
    {
        public EquipmentItemDTO Weapon;
        public EquipmentItemDTO Armor;
        public EquipmentItemDTO[] Artifacts;
        public EquipmentItemDTO[] Consumables;
        public EquipmentItemDTO[] Backpack;

        public HeroEquipmentSelectionDTO(EquipmentItemDTO weapon, EquipmentItemDTO armor, EquipmentItemDTO[] artifacts, EquipmentItemDTO[] consumables, EquipmentItemDTO[] backpack)
        {
            Weapon = weapon;
            Armor = armor;
            Artifacts = artifacts ?? Array.Empty<EquipmentItemDTO>();
            Consumables = consumables ?? Array.Empty<EquipmentItemDTO>();
            Backpack = backpack ?? Array.Empty<EquipmentItemDTO>();
        }
    }

    public class ShopItemSelectionDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public string Icon;
        public int GoldCost;
        public float Attack;
        public float Defense;
        public float Speed;
        public float HpRegeneration;
        public string Slot;
        public bool IsSingleUse;
        public bool IsUnlocked;
        public string LockReason;

        public ShopItemSelectionDTO(
            string id,
            string name,
            string description,
            string icon,
            int goldCost,
            float attack,
            float defense,
            float speed,
            float hpRegeneration,
            string slot,
            bool isSingleUse,
            bool isUnlocked,
            string lockReason)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
            GoldCost = goldCost;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            HpRegeneration = hpRegeneration;
            Slot = slot;
            IsSingleUse = isSingleUse;
            IsUnlocked = isUnlocked;
            LockReason = lockReason;
        }
    }
    
    public class GoapGoalSelectionDTO
    {
        public string Name;
        public string Description;
        public string Icon;
        public float Value;
        public bool IsActive;

        public GoapGoalSelectionDTO(string name, string description, string icon, float value, bool isActive)
        {
            Name = name;
            Description = description;
            Value = value;
            Icon = icon;
            IsActive = isActive;
        }
    }

    public class GoapPlanStepSelectionDTO
    {
        public string Name;
        public string Description;
        public string Icon;
        public bool PreconditionsMet;

        public GoapPlanStepSelectionDTO(string name, string description, string icon, bool preconditionsMet)
        {
            Name = name;
            Description = description;
            Icon = icon;
            PreconditionsMet = preconditionsMet;
        }
    }

    public class GoapSelectionDTO
    {
        public GoapPlanStepSelectionDTO[] Steps;
        public GoapGoalSelectionDTO[] Goals;
        public bool IsThinking;

        public GoapSelectionDTO(GoapGoalSelectionDTO[] goals, GoapPlanStepSelectionDTO[] steps, bool isThinking)
        {
            Goals = goals ?? Array.Empty<GoapGoalSelectionDTO>();
            Steps = steps ?? Array.Empty<GoapPlanStepSelectionDTO>();
            IsThinking = isThinking;
        }
    }

    public class BuildingUpgradeSelectionDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public int Price;
        public string Icon;
        public bool IsQueued;
        public bool IsActive;
        public bool IsCompleted;
        public bool CanQueue;
        public string LockReason;
        public float Progress;
        public int QueueIndex;

        public BuildingUpgradeSelectionDTO(string id, string name, string description, int price, string icon, bool isQueued, bool isActive, bool isCompleted, bool canQueue, string lockReason, float progress, int queueIndex)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Icon = icon;
            IsQueued = isQueued;
            IsActive = isActive;
            IsCompleted = isCompleted;
            CanQueue = canQueue;
            LockReason = lockReason;
            Progress = Math.Clamp(progress, 0f, 1f);
            QueueIndex = queueIndex;
        }
    }

    public class QueuedBuildingUpgradeSelectionDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public string Icon;
        public int Order;

        public QueuedBuildingUpgradeSelectionDTO(string id, string name, string description, string icon, int order)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
            Order = order;
        }
    }
}


