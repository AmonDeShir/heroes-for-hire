using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Buildings;
using UnityEngine;
using EntityId = Heroes.Game.Core.EntityId;

namespace Heroes.Game.Abstractions
{
    public interface IBuilding : IHasHealth
    {
        public EntityId Id { get; }
        public bool IsUnderConstruction { get; }
        public IBuildingDefinition Definition { get; }
        public Vector2 Position { get; }
        public BuildingCategory Category => Definition.Category;
        public string Name => Definition.DisplayName;
    }
}
