using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Buildings;
using Heroes.Game.Abstractions;
using Heroes.Game.Abstractions.Common;
using Heroes.Game.Domain.Common;
using UnityEngine;
using EntityId = Heroes.Game.Core.EntityId;

namespace Heroes.Game.Domain.Buildings
{
    public class Building : IBuilding
    {
        public EntityId Id { get; }
        public IHealthComponent Health { get; }
        public bool IsUnderConstruction { get; private set; }

        public IBuildingDefinition Definition { get; }
        public Vector2 Position { get; }

        public BuildingCategory Category => Definition.Category;
        public string Name => Definition.DisplayName;

        public Building(
            EntityId id,
            IBuildingDefinition definition,
            Vector2 position,
            bool isUnderConstruction)
        {
            Id = id;
            Definition = definition;
            Position = position;
            IsUnderConstruction = isUnderConstruction;
            Health = new HealthComponent(
                definition.MaxHealth,
                definition.SpawnHealth,
                definition.BaseRegeneration);
        }
    }
}
