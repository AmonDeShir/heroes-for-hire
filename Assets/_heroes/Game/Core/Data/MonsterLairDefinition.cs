using Heroes.Content.Monsters;
using Heroes.Game.Monsters;
using UnityEngine;

namespace Heroes.Content.Monsters
{
    [CreateAssetMenu(menuName = "Heroes/Monsters/Monster Lair Definition")]
    public sealed class MonsterLairDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string DisplayName;

        [Multiline]
        public string Description;

        [ResourceIcon("Buildings")]
        public string IconPath;

        [Header("Monster")]
        public MonsterDefinition Monster;

        [Header("Waves")]
        public float SpawnIntervalSeconds = 60f;
        public int MaxAlive = 20;
        public int RaidThreshold = 10;

        public MonsterLairSpawner.WaveMilestone[] Milestones = new MonsterLairSpawner.WaveMilestone[]
        {
            new MonsterLairSpawner.WaveMilestone { Minutes = 0f, Count = 1 },
            new MonsterLairSpawner.WaveMilestone { Minutes = 15f, Count = 2 },
            new MonsterLairSpawner.WaveMilestone { Minutes = 30f, Count = 3 },
            new MonsterLairSpawner.WaveMilestone { Minutes = 45f, Count = 3 },
            new MonsterLairSpawner.WaveMilestone { Minutes = 60f, Count = 4 },
            new MonsterLairSpawner.WaveMilestone { Minutes = 90f, Count = 10 },
        };
    }
}
