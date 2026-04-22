using EventBus;
using Heroes.Game.Core.Events;

namespace Heroes.Game.Buildings
{
    public sealed class BuildingPlacementSelectionService
    {
        public string Selected { get; private set; }

        public void Select(string buildingId)
        {
            if (Selected == buildingId)
            {
                return;
            }

            Selected = buildingId;
            EventBus<BuildingPlacementSelectedChangedEvent>.Invoke(new BuildingPlacementSelectedChangedEvent
            {
                Value = Selected
            });
        }

        public void Clear()
        {
            if (string.IsNullOrEmpty(Selected))
            {
                return;
            }

            Selected = null;
            EventBus<BuildingPlacementSelectedChangedEvent>.Invoke(new BuildingPlacementSelectedChangedEvent
            {
                Value = Selected
            });
        }
    }
}
