using Heroes.Game.Monsters;
using UnityEngine;

namespace Heroes.Content.Monsters
{
    [CreateAssetMenu(menuName = "Heroes/Monsters/Monster Definition")]
    public sealed class MonsterDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string DisplayName;

        [Multiline]
        public string Description;

        [ResourceIcon("Monsters")]
        public string IconPath;

        [Header("Prefab")]
        public MonsterFacade Prefab;

        [Header("Stats")]
        public float MaxHp = 100f;
        public float MoveSpeed = 3.5f;
        public float AttackDamage = 5f;
        public float AttackIntervalSeconds = 1.0f;
        public float AttackRange = 1.6f;
        public float AggroRange = 12f;
        public float WanderRadius = 12f;

        [Header("AI")]
        [Tooltip("If current HP% falls below this value, monster will try to flee to spawn.")]
        [Range(0f, 1f)]
        public float FleeHpPct = 0.2f;

        [Header("Loot")]
        public int GoldMin = 50;
        public int GoldMax = 150;
    }
}
