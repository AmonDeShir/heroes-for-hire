using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Effects;
using Heroes.Content.Definitions.Entities;
using Heroes.Content.Definitions.Items;
using Heroes.Content.Definitions.Skills;

namespace Heroes.Editor.ContentEditor.Validation
{
    public static class ContentValidationRegistry
    {
        private static readonly List<IContentValidator> Validators = new()
        {
            new DefinitionBaseValidator(),
            new BuildingDefinitionValidator(),
            new BuildingUpgradeValidator(),
            new SkillDefinitionValidator(),
            new EffectDefinitionValidator(),
            new ItemDefinitionValidator(),
            new EntityDefinitionValidator(),
            new HeroDefinitionValidator(),
        };

        public static IReadOnlyList<ContentValidationResult> Validate(DefinitionBase asset)
        {
            if (asset == null)
            {
                return System.Array.Empty<ContentValidationResult>();
            }

            var results = new List<ContentValidationResult>();
            foreach (var validator in Validators)
            {
                if (!validator.CanValidate(asset))
                {
                    continue;
                }

                results.AddRange(validator.Validate(asset));
            }

            return results;
        }

        private sealed class DefinitionBaseValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset != null;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                if (string.IsNullOrWhiteSpace(asset.Id))
                {
                    yield return new ContentValidationResult("Missing Id", ContentValidationSeverity.Error);
                }

                if (string.IsNullOrWhiteSpace(asset.DisplayName))
                {
                    yield return new ContentValidationResult("Missing Display Name", ContentValidationSeverity.Warning);
                }

                if (string.IsNullOrWhiteSpace(asset.DisplayNameKey))
                {
                    yield return new ContentValidationResult("Missing DisplayNameKey", ContentValidationSeverity.Warning);
                }

                if (string.IsNullOrWhiteSpace(asset.DescriptionKey))
                {
                    yield return new ContentValidationResult("Missing DescriptionKey", ContentValidationSeverity.Info);
                }
            }
        }

        private sealed class BuildingDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is BuildingDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var building = asset as BuildingDefinition;
                if (building == null)
                {
                    yield break;
                }

                if (building.Prefab == null)
                {
                    yield return new ContentValidationResult("Missing main prefab", ContentValidationSeverity.Warning);
                }

                if (building.Prefab == null)
                {
                    yield return new ContentValidationResult("Missing main prefab", ContentValidationSeverity.Warning);
                }
            }
        }

        private sealed class BuildingUpgradeValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is BuildingUpgradeDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var upgrade = asset as BuildingUpgradeDefinition;
                if (upgrade == null)
                {
                    yield break;
                }

                if (upgrade.TargetLevel <= 0)
                {
                    yield return new ContentValidationResult("TargetLevel should be > 0", ContentValidationSeverity.Warning);
                }
            }
        }

        private sealed class SkillDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is SkillDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var skill = asset as SkillDefinition;
                if (skill == null)
                {
                    yield break;
                }

                if (skill.BaseDamage <= 0f && (skill.Effects == null || skill.Effects.Count == 0))
                {
                    yield return new ContentValidationResult("Skill has no base damage or effects", ContentValidationSeverity.Warning);
                }
            }
        }

        private sealed class EffectDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is EffectDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var effect = asset as EffectDefinition;
                if (effect == null)
                {
                    yield break;
                }

                if (effect.Type == EffectType.Buff || effect.Type == EffectType.Debuff)
                {
                    if (effect.Modifiers == null || effect.Modifiers.Count == 0)
                    {
                        yield return new ContentValidationResult("Buff/Debuff has no modifiers", ContentValidationSeverity.Warning);
                    }
                }
            }
        }

        private sealed class ItemDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is ItemDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var item = asset as ItemDefinition;
                if (item == null)
                {
                    yield break;
                }

                if ((item.Effects == null || item.Effects.Count == 0) &&
                    (item.GrantedSkills == null || item.GrantedSkills.Count == 0))
                {
                    yield return new ContentValidationResult("Item has no effects or skills", ContentValidationSeverity.Warning);
                }
            }
        }

        private sealed class EntityDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is EntityDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var entity = asset as EntityDefinition;
                if (entity == null)
                {
                    yield break;
                }

                if (entity.DefaultAttackSkill == null)
                {
                    yield return new ContentValidationResult("Missing default attack skill", ContentValidationSeverity.Info);
                }
            }
        }

        private sealed class HeroDefinitionValidator : IContentValidator
        {
            public bool CanValidate(DefinitionBase asset) => asset is HeroDefinition;

            public IEnumerable<ContentValidationResult> Validate(DefinitionBase asset)
            {
                var hero = asset as HeroDefinition;
                if (hero == null)
                {
                    yield break;
                }

                if (hero.MaxArtifactSlots <= 0)
                {
                    yield return new ContentValidationResult("MaxArtifactSlots should be > 0", ContentValidationSeverity.Warning);
                }
            }
        }
    }
}
