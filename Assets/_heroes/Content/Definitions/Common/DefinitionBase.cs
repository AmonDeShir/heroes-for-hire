using Heroes.Content.Abstractions;
using UnityEngine;

namespace Heroes.Content.Definitions.Common
{
    public abstract class DefinitionBase : ScriptableObject, IDefinition, ILocalizedDefinition, IIconDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private string displayNameKey;
        [SerializeField] private string descriptionKey;
        [SerializeField] private string iconResourcePath;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public string DisplayNameKey => displayNameKey;
        public string DescriptionKey => descriptionKey;
        public string IconResourcePath => iconResourcePath;
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (!UnityEditor.EditorUtility.IsPersistent(this))
            {
                return;
            }

            DefinitionLocalizationUtility.EnsureLocalizationEntries(this);
        }
#endif
    }
}
