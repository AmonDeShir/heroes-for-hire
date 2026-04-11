using Heroes.Content.Buildings;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public sealed class BuildingDTO
    {
        public string Id;
        public string Name;
        public string Description;
        public int Price;
        public string Icon;
        public string Category;

        public BuildingDTO(BuildingDefinition definition)
        {
            Id = definition.Id;
            Name = definition.DisplayName;
            Description = definition.Description;
            Price = definition.GoldCost;
            Icon = definition.IconPath;
            Category = definition.Category.ToString();
        }
    }
}
