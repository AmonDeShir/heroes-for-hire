using System.Collections.Generic;
using Heroes.Game.Domain.Buildings;
using UnityEngine;

namespace Heroes.Game.Systems.Buildings
{
    public interface IBuildingSystem
    {
        IReadOnlyList<Building> Buildings { get; }
        bool TryPlaceSelectedBuilding(Vector2 position);
        bool HasBuildingOfType(Content.Definitions.Buildings.BuildingType type);
    }
}