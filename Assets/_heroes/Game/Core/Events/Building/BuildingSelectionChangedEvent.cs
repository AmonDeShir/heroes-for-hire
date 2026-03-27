namespace Heroes.Game.Core.Events
{
    public readonly struct BuildingSelectionChangedEvent
    {
        public string SelectedBuildingDefinitionId { get; }

        public BuildingSelectionChangedEvent(string selectedBuildingDefinitionId)
        {
            SelectedBuildingDefinitionId = selectedBuildingDefinitionId;
        }
    }
}