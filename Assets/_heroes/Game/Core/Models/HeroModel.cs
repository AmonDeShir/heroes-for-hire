using EventBus;
using Heroes.Content.Heroes;
using Heroes.Game.Core.Events;
using UnityEngine;
using System.Collections.Generic;

namespace Heroes.Game.Heroes
{
    public sealed class HeroModel
    {
        public string InstanceId { get; }
        public string DefinitionId { get; }
        public string HomeBuildingInstanceId { get; }
        public Core.Health.HealthModel Health { get; }

        public int Gold { get; private set; }
        public float GearLevel { get; private set; }
        public float DangerLevel { get; private set; }

        
        public int WeaponTier { get; private set; }
        public int ArmorTier { get; private set; }
        public int AmuletTier { get; private set; }

        public float EquipmentAttack { get; private set; }
        public float EquipmentDefence { get; private set; }
        public float EquipmentSpeed { get; private set; }
        public float EquipmentHpRegeneration { get; private set; }

        public float TimedAttack { get; private set; }
        public float TimedDefence { get; private set; }
        public float TimedSpeed { get; private set; }

        public float TimedHpRegeneration { get; private set; }

        
        public string EquippedWeaponId { get; private set; }
        public float EquippedWeaponPower { get; private set; }
        public float EquippedWeaponAttack { get; private set; }
        public float EquippedWeaponDefence { get; private set; }
        public float EquippedWeaponSpeed { get; private set; }
        public float EquippedWeaponHpRegeneration { get; private set; }
        public string EquippedArmorId { get; private set; }
        public float EquippedArmorPower { get; private set; }
        public float EquippedArmorAttack { get; private set; }
        public float EquippedArmorDefence { get; private set; }
        public float EquippedArmorSpeed { get; private set; }
        public float EquippedArmorHpRegeneration { get; private set; }

        public IReadOnlyList<string> EquippedArtifacts => _artifacts;
        public IReadOnlyList<string> EquippedConsumables => _consumables;
        public IReadOnlyList<string> Backpack => _backpack;

        private readonly List<string> _artifacts = new(capacity: 3);
        private readonly List<string> _consumables = new(capacity: 3);
        private readonly List<string> _backpack = new(capacity: 5);
        public bool IsAlive => Health.Current > 0f;
        public bool IsInHome { get; private set; }
        public float HomeRadius { get; }
        public float DangerSenseRadius { get; }

        public string DefendBuildingInstanceId { get; private set; }
        public float DefendBuildingUntilTime { get; private set; }

        public string ActiveQuestId { get; private set; }
        public string ActiveQuestTargetInstanceId { get; private set; }
        public Quests.QuestTargetKind ActiveQuestTargetKind { get; private set; }

        public HeroModel(string instanceId, HeroDefinition definition, string homeBuildingInstanceId)
        {
            InstanceId = instanceId;
            DefinitionId = definition != null ? definition.Id : string.Empty;
            HomeBuildingInstanceId = homeBuildingInstanceId;
            HomeRadius = definition != null ? Mathf.Max(0.1f, definition.HomeRadius) : 2f;
            DangerSenseRadius = definition != null ? Mathf.Max(1f, definition.DangerSenseRadius) : 12f;
            Health = new Core.Health.HealthModel(instanceId, definition != null ? definition.MaxHp : 0f, definition != null ? definition.StartHp : 0f);
            Gold = definition != null ? definition.StartGold : 0;
            GearLevel = definition != null ? definition.BaseGearLevel : 0f;
            DangerLevel = 0f;

            DefendBuildingInstanceId = string.Empty;
            DefendBuildingUntilTime = 0f;

            ActiveQuestId = string.Empty;
            ActiveQuestTargetInstanceId = string.Empty;
            ActiveQuestTargetKind = Quests.QuestTargetKind.Building;

            WeaponTier = 0;
            ArmorTier = 0;
            AmuletTier = 0;

            EquipmentAttack = 0f;
            EquipmentDefence = 0f;
            EquipmentSpeed = 0f;
            EquipmentHpRegeneration = 0f;

            TimedAttack = 0f;
            TimedDefence = 0f;
            TimedSpeed = 0f;
            TimedHpRegeneration = 0f;

            EquippedWeaponId = string.Empty;
            EquippedWeaponPower = 0f;
            EquippedWeaponAttack = 0f;
            EquippedWeaponDefence = 0f;
            EquippedWeaponSpeed = 0f;
            EquippedWeaponHpRegeneration = 0f;
            EquippedArmorId = string.Empty;
            EquippedArmorPower = 0f;
            EquippedArmorAttack = 0f;
            EquippedArmorDefence = 0f;
            EquippedArmorSpeed = 0f;
            EquippedArmorHpRegeneration = 0f;

            
            if (definition != null && definition.StartingItems != null)
            {
                for (var i = 0; i < definition.StartingItems.Length; i++)
                {
                    TryAddAndAutoEquip(definition.StartingItems[i]);
                }
            }
        }

        public bool TryAddAndAutoEquip(ItemDefinition item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
            {
                return false;
            }

            
            const int maxArtifacts = 3;
            const int maxConsumables = 3;
            const int maxBackpack = 5;

            var power = GetItemPower(item);
            var atk = item.Attack;
            var def = item.Defense;
            var spd = item.Speed;
            var regen = item.HpRegeneration;

            switch (item.Slot)
            {
                case EquipmentSlot.Weapon:
                {
                    
                    if (string.IsNullOrWhiteSpace(EquippedWeaponId) || power > EquippedWeaponPower)
                    {
                        EquipmentAttack -= EquippedWeaponAttack;
                        EquipmentDefence -= EquippedWeaponDefence;
                        EquipmentSpeed -= EquippedWeaponSpeed;
                        EquipmentHpRegeneration -= EquippedWeaponHpRegeneration;

                        EquippedWeaponId = item.Id;
                        EquippedWeaponPower = power;
                        WeaponTier = Mathf.Max(0, item.Tier);

                        EquippedWeaponAttack = atk;
                        EquippedWeaponDefence = def;
                        EquippedWeaponSpeed = spd;
                        EquippedWeaponHpRegeneration = regen;

                        EquipmentAttack += EquippedWeaponAttack;
                        EquipmentDefence += EquippedWeaponDefence;
                        EquipmentSpeed += EquippedWeaponSpeed;
                        EquipmentHpRegeneration += EquippedWeaponHpRegeneration;

                        BumpGearFromPower();
                        return true;
                    }

                    return TryAddToBackpack(item.Id, maxBackpack);
                }

                case EquipmentSlot.Armor:
                {
                    if (string.IsNullOrWhiteSpace(EquippedArmorId) || power > EquippedArmorPower)
                    {
                        EquipmentAttack -= EquippedArmorAttack;
                        EquipmentDefence -= EquippedArmorDefence;
                        EquipmentSpeed -= EquippedArmorSpeed;
                        EquipmentHpRegeneration -= EquippedArmorHpRegeneration;

                        EquippedArmorId = item.Id;
                        EquippedArmorPower = power;
                        ArmorTier = Mathf.Max(0, item.Tier);

                        EquippedArmorAttack = atk;
                        EquippedArmorDefence = def;
                        EquippedArmorSpeed = spd;
                        EquippedArmorHpRegeneration = regen;

                        EquipmentAttack += EquippedArmorAttack;
                        EquipmentDefence += EquippedArmorDefence;
                        EquipmentSpeed += EquippedArmorSpeed;
                        EquipmentHpRegeneration += EquippedArmorHpRegeneration;

                        BumpGearFromPower();
                        return true;
                    }

                    return TryAddToBackpack(item.Id, maxBackpack);
                }

                case EquipmentSlot.Item:
                default:
                {
                    
                    if (item.IsSingleUse)
                    {
                        if (_consumables.Count < maxConsumables)
                        {
                            _consumables.Add(item.Id);
                            EquipmentAttack += atk;
                            EquipmentDefence += def;
                            EquipmentSpeed += spd;
                            EquipmentHpRegeneration += regen;
                            BumpGearFromPower();
                            return true;
                        }

                        return TryAddToBackpack(item.Id, maxBackpack);
                    }

                    if (_artifacts.Count < maxArtifacts)
                    {
                        _artifacts.Add(item.Id);
                        
                        AmuletTier = Mathf.Max(AmuletTier, Mathf.Max(0, item.Tier));
                        EquipmentAttack += atk;
                        EquipmentDefence += def;
                        EquipmentSpeed += spd;
                        EquipmentHpRegeneration += regen;
                        BumpGearFromPower();
                        return true;
                    }

                    return TryAddToBackpack(item.Id, maxBackpack);
                }
            }
        }

        private bool TryAddToBackpack(string itemId, int maxBackpack)
        {
            if (_backpack.Count >= maxBackpack)
            {
                return false;
            }

            _backpack.Add(itemId);
            return true;
        }

        public bool RemoveConsumable(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            var idx = _consumables.IndexOf(itemId);
            if (idx < 0)
            {
                return false;
            }

            _consumables.RemoveAt(idx);
            return true;
        }

        private static float GetItemPower(ItemDefinition item)
        {
            
            return item.Attack + item.Defense + item.Speed;
        }

        private void BumpGearFromPower()
        {
            
            
            GearLevel += 0.1f;
            if (GearLevel < 0f)
            {
                GearLevel = 0f;
            }
        }

        public void SetGold(int value)
        {
            Gold = value < 0 ? 0 : value;
            EventBus<HeroGoldChangedEvent>.Invoke(new HeroGoldChangedEvent { Id = InstanceId, Value = Gold });
        }

        public void SetGearLevel(float value)
        {
            GearLevel = value < 0f ? 0f : value;
        }

        public void SetDangerLevel(float value)
        {
            DangerLevel = 0f;
        }

        public void SetDefendBuilding(string instanceId, float untilTime)
        {
            DefendBuildingInstanceId = string.IsNullOrWhiteSpace(instanceId) ? string.Empty : instanceId;
            DefendBuildingUntilTime = untilTime;
        }

        public void SetActiveQuest(string questId, string targetInstanceId, Quests.QuestTargetKind kind)
        {
            ActiveQuestId = questId ?? string.Empty;
            ActiveQuestTargetInstanceId = targetInstanceId ?? string.Empty;
            ActiveQuestTargetKind = kind;
        }

        public void ClearActiveQuest(string questId)
        {
            if (!string.IsNullOrWhiteSpace(questId) && questId != ActiveQuestId)
            {
                return;
            }

            ActiveQuestId = string.Empty;
            ActiveQuestTargetInstanceId = string.Empty;
        }

        public void SetInHome(bool value)
        {
            IsInHome = value;
            if (value)
            {
                SetDangerLevel(0f);
            }
        }

        public void SetTimedBonuses(float attack, float defence, float speed)
        {
            TimedAttack = attack;
            TimedDefence = defence;
            TimedSpeed = speed;
        }

        public void SetTimedHpRegeneration(float value)
        {
            TimedHpRegeneration = value;
        }
    }
}


