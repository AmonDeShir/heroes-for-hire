using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Content.Heroes
{
    [CreateAssetMenu(menuName = "Heroes/Heroes/Hero Definition")]
    public class HeroDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID] 
        public string Id;
        public string DisplayName;
        
        [Multiline] 
        public string Description;
        
        [ResourceIcon("Heros")]
        public string IconPath;

        [Header("Stats")] 
        public float MaxHp = 100f;
        public float StartHp = 100f;
        public float HpRegeneration = 1f;
        public float Attack;
        public float Defence;
        public float Speed;
        
        [Header("Other Stats")]
        public int StartGold;
        public float BaseGearLevel;
        public float HomeRadius = 2f;
        public float DangerSenseRadius = 12f;
        
        
        [Header("Prefab")]
        public HeroFacade Prefab;
    }
}
