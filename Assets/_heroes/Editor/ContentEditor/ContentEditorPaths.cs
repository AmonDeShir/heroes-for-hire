using System;
using System.Collections.Generic;
using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Definitions.Effects;
using Heroes.Content.Definitions.Entities;
using Heroes.Content.Definitions.Items;
using Heroes.Content.Definitions.Skills;

namespace Heroes.Editor.ContentEditor
{
    public static class ContentEditorPaths
    {
        public const string BuildingsFolder = "Assets/_heroes/Content/Data/Buildings";
        public const string UpgradesFolder = "Assets/_heroes/Content/Data/Upgrades";
        public const string EntitiesFolder = "Assets/_heroes/Content/Data/Entities";
        public const string HeroesFolder = "Assets/_heroes/Content/Data/Heroes";
        public const string ItemsFolder = "Assets/_heroes/Content/Data/Items";
        public const string SkillsFolder = "Assets/_heroes/Content/Data/Skills";
        public const string EffectsFolder = "Assets/_heroes/Content/Data/Effects";
        public const string IconsFolder = "Assets/Resources/Icons";

        public static readonly IReadOnlyDictionary<Type, string> PathsByType =
            new Dictionary<Type, string>
            {
                { typeof(BuildingDefinition), BuildingsFolder },
                { typeof(BuildingUpgradeDefinition), UpgradesFolder },
                { typeof(EntityDefinition), EntitiesFolder },
                { typeof(HeroDefinition), HeroesFolder },
                { typeof(ItemDefinition), ItemsFolder },
                { typeof(SkillDefinition), SkillsFolder },
                { typeof(EffectDefinition), EffectsFolder },
            };
    }
}
