using Heroes.Content.Abstractions;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public struct BuildingDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public int Price;
        public string Icon;
        public string Category;

        public BuildingDTO(IBuildingDefinition definition)
        {
            Id = definition.Id;
            Name = definition.DisplayName;
            Description = definition.Description;
            Price = definition.GoldCost;
            Icon = definition.IconResourcePath;
            Category = definition.Category.ToString();
        }
    }
}