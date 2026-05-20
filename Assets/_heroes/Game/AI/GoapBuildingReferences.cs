using Heroes.Content.Buildings;
using UnityEngine;

namespace Heroes.Game.AI
{
    [CreateAssetMenu(menuName = "Heroes/AI/GOAP Building References")]
    public sealed class GoapBuildingReferences : ScriptableObject
    {
        [Header("Debug")]
        public bool EnableGoapDebugLogs;

        [Header("Core")]
        public BuildingDefinition Castle;
        public BuildingDefinition House;
        public BuildingDefinition Farm;
        public BuildingDefinition Tower;

        [Header("Economy")]
        public BuildingDefinition Market;
        public BuildingDefinition Blacksmith;
        public BuildingDefinition Alchemist;
        public BuildingDefinition Chapel;

        [Header("Heroes")]
        public BuildingDefinition Guild;
    }
}


