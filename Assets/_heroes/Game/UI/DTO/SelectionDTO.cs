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
