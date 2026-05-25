using Heroes.Content.Buildings;
using Heroes.Content.Monsters;
using UnityEngine;

namespace Heroes.Game.Monsters
{
    public sealed class MonsterLairSpawnPoint : MonoBehaviour
    {
        [SerializeField] private BuildingDefinition buildingDefinition;
        [SerializeField] private MonsterLairDefinition lairDefinition;
        [SerializeField] private Transform monsterSpawnPoint;
        [SerializeField] private bool spawnOnStart = true;

        public BuildingDefinition BuildingDefinition => buildingDefinition;
        public MonsterLairDefinition LairDefinition => lairDefinition;
        public Transform MonsterSpawnPoint => monsterSpawnPoint;
        public bool SpawnOnStart => spawnOnStart;
    }
}
