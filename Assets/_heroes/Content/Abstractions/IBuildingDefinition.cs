using Heroes.Content.Definitions.Buildings;
using UnityEngine;

namespace Heroes.Game.Abstractions
{
    public interface IBuildingDefinition: IHealthDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public BuildingType Type { get; }
        public int GoldCost { get; }
        
        public GameObject Prefab { get; }
    }
}