using System.Collections.Generic;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Entities;
using Heroes.Content.Definitions.Items;
using Heroes.Content.Definitions.Skills;
using Heroes.Content.Abstractions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Heroes.Content.Definitions.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Definition")]
    public class BuildingDefinition : DefinitionBase, IBuildingDefinition
    {
        [FormerlySerializedAs("buildingType")]
        [SerializeField] private BuildingCategory buildingCategory;
        [SerializeField] private int goldCost;
        [SerializeField] private GameObject prefab;

        [SerializeField] private BuildingGoldGenerationDefinition goldGeneration;
        [SerializeField] private List<BuildingAutoSpawnDefinition> autoSpawns = new();
        [SerializeField] private BuildingAttackDefinition attack;
        [SerializeField] private BuildingHeroRecruitmentDefinition heroRecruitment;
        [SerializeField] private BuildingShopDefinition shop;
        [SerializeField] private BuildingUnlocksDefinition unlocks;
        [SerializeField] private List<BuildingUpgradeDefinition> upgrades = new();
        [SerializeField] private BuildingQueueDefinition queue = new() { };
        
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float spawnHealth = 10f;
        [SerializeField] private float baseRegeneration = 1f;
        
        public BuildingCategory Category => buildingCategory;
        public int GoldCost => goldCost;
        
        public GameObject Prefab => prefab;
        public IBuildingGoldGenerationDefinition GoldGeneration => goldGeneration;
        public IReadOnlyList<IBuildingAutoSpawnDefinition> AutoSpawns =>
            DefinitionListUtility.ToInterfaceList<BuildingAutoSpawnDefinition, IBuildingAutoSpawnDefinition>(autoSpawns);
        public IBuildingAttackDefinition Attack => attack;
        public IBuildingHeroRecruitmentDefinition HeroRecruitment => heroRecruitment;
        public IBuildingShopDefinition Shop => shop;
        public IBuildingUnlocksDefinition Unlocks => unlocks;
        public IReadOnlyList<IBuildingUpgradeDefinition> Upgrades =>
            DefinitionListUtility.ToInterfaceList<BuildingUpgradeDefinition, IBuildingUpgradeDefinition>(upgrades);
        public IBuildingQueueDefinition Queue => queue;
        
        public float MaxHealth => maxHealth;
        public float SpawnHealth => spawnHealth;
        public float BaseRegeneration => baseRegeneration;

    }
}
