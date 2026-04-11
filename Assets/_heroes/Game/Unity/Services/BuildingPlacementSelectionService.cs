using System;

namespace Heroes.Game.Buildings
{
    public sealed class BuildingPlacementSelectionService
    {
        public event Action<string> OnSelectedChanged;

        public string Selected { get; private set; }

        public void Select(string buildingId)
        {
            if (Selected == buildingId)
            {
                return;
            }

            Selected = buildingId;
            OnSelectedChanged?.Invoke(Selected);
        }

        public void Clear()
        {
            if (string.IsNullOrEmpty(Selected))
            {
                return;
            }

            Selected = null;
            OnSelectedChanged?.Invoke(Selected);
        }
    }
}
