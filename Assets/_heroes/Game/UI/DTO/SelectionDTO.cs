using System;

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

        public BuildingSelectionDTO(bool isAlive)
        {
            IsAlive = isAlive;
        }
    }

    public class HeroSelectionDTO
    {
        public int Gold;
        public float GearLevel;
        public float DangerLevel;
        public bool IsAlive;
        public bool IsInHome;

        public HeroSelectionDTO(int gold, float gearLevel, float dangerLevel, bool isAlive, bool isInHome)
        {
            Gold = gold;
            GearLevel = gearLevel;
            DangerLevel = dangerLevel;
            IsAlive = isAlive;
            IsInHome = isInHome;
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
        public bool PreconditionsMet;

        public GoapPlanStepSelectionDTO(string name, string description, bool preconditionsMet)
        {
            Name = name;
            Description = description;
            PreconditionsMet = preconditionsMet;
        }
    }

    public class GoapSelectionDTO
    {
        public GoapPlanStepSelectionDTO[] Steps;
        public GoapGoalSelectionDTO[] Goals;

        public GoapSelectionDTO(GoapGoalSelectionDTO[] goals, GoapPlanStepSelectionDTO[] steps)
        {
            Goals = goals ?? Array.Empty<GoapGoalSelectionDTO>();
            Steps = steps ?? Array.Empty<GoapPlanStepSelectionDTO>();
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
