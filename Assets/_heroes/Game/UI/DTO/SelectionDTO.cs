using Heroes.Content.Buildings;
using Heroes.Game.Core.Health;
using JetBrains.Annotations;

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
        public bool IsCompleted;

        public BuildingSelectionDTO(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }
    }
}
