using Heroes.Content.Buildings;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public sealed class BuildingDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public int Price;
        public int PopulationCost;
        public bool CanBuild;
        public string LockReason;
        public string Icon;
        public string Category;

        public BuildingDTO(BuildingDefinition definition, bool canBuild, string lockReason)
        {
            Id = definition.Id;
            Name = definition.DisplayName;
            Description = definition.Description;
            Price = definition.GoldCost;
            PopulationCost = definition.PopulationCost;
            CanBuild = canBuild;
            LockReason = lockReason;
            Icon = definition.IconPath;
            Category = definition.Category.ToString();
        }
    }
}


