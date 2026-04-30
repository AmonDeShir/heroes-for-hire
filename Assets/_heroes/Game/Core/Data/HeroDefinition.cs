using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Content.Heroes
{
    [CreateAssetMenu(menuName = "Heroes/Heroes/Hero Definition")]
    public class HeroDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id;
        public string DisplayName;
        public string Description;
        public string IconPath;

        [Header("Stats")]
        public float MaxHp = 100f;
        public float StartHp = 100f;
        public int StartGold;
        public float BaseGearLevel;
        public float HomeRadius = 2f;
        public float DangerSenseRadius = 12f;

        [Header("Prefab")]
        public HeroFacade Prefab;
    }
}
