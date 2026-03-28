using UnityEngine;

namespace Heroes.Game.Abstractions
{
    public interface IBuildingPlacementService
    {
        bool TryPlaceSelectedBuilding(Vector2 position);
    }
}
