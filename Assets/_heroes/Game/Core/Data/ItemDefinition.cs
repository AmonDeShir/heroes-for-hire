using UnityEngine;

namespace Heroes.Content.Heroes
{
    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string DisplayName;
        
        [Multiline]
        public string Description;
        
        [ResourceIcon("Items")]
        public string IconPath;   
    }
}