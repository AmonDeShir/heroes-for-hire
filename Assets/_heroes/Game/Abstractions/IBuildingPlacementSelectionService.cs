using System;

namespace Heroes.Game.Abstractions
{
    public interface IBuildingPlacementSelectionService
    {
        string SelectedBuildingDefinitionId { get; }
        void Select(string buildingDefinitionId);
        void Clear();
        
        event Action<string> OnSelectedChanged;
    }
}
