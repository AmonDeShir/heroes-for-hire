using Heroes.Game.Abstractions;
using UnityEngine;

namespace Heroes.Content.Definitions.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Definition")]
    public class BuildingDefinition : ScriptableObject, IBuildingDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private BuildingType buildingType;
        [SerializeField] private int goldCost;
        [SerializeField] private GameObject prefab;
        
        [SerializeField] private float maxHealth;
        [SerializeField] private float spawnHealth;
        [SerializeField] private float baseRegeneration;
        
        public string Id => id;
        public string DisplayName => displayName;
        public BuildingType Type => buildingType;
        public int GoldCost => goldCost;
        
        public GameObject Prefab => prefab;
        
        public float MaxHealth => maxHealth;
        public float SpawnHealth => spawnHealth;
        public float BaseRegeneration => baseRegeneration;
    }
}