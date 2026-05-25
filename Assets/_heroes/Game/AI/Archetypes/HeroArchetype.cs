using System.Collections.Generic;
using System.Linq;
using Heroes.Game.Heroes;
using Heroes.Game.AI.Strategies;
using Heroes.GOAP.Core;
using Heroes.GOAP;
using UnityEngine;
using Heroes.Content.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Buildings;
using Heroes.Game.Monsters;
using Heroes.Game.Runtime;
using Heroes.Game.Core.Events;
using Heroes.Game.Quests;
using Registry;

namespace Heroes.Game.AI
{
        public class HeroArchetype :  Archetype<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>
        {
            private const float WanderRadius = 6f;
            private const float HuntRadius = 80f;
            private const int DesiredConsumables = 3;
            private const int HuntExpectedGold = 50;
        private readonly string _homeBuildingInstanceId;
        private readonly GoapBuildingReferences _buildings;
        private readonly HeroFacade _hero;
        private float _nextDebugLogAt;
        private string _lastDebugReason;

        public HeroArchetype(HeroFacade hero, GoapBuildingReferences buildings) : base(
            new List<Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>>(),
            new List<Goal<GameWorldSnapshot>>(),
            CreateBaseState(hero))
        {
            _hero = hero;
            _homeBuildingInstanceId = hero?.Model?.HomeBuildingInstanceId ?? string.Empty;
            _buildings = buildings;
            CreateBelieves();
            CreateGoals();
            CreateActions();
            CreateWanderIdleAction();
        }

        private static AgentState CreateBaseState(HeroFacade hero)
        {
            var state = new AgentState(Consts.BELIEF_COUNT);
            

            if (hero?.Model != null)
            {
                state.SetLocation(hero.transform.position);
                state.SetBelieve(Consts.GOLD, hero.Model.Gold);
                state.SetBelieve(Consts.HEALTH, hero.Model.Health.Current);
                var maxHp = hero.Model.Health.Max;
                state.SetBelieve(Consts.HEALTH_PCT, maxHp > 0.001f ? hero.Model.Health.Current / maxHp : 1f);
                state.SetBelieve(Consts.ENEMIES_NEARBY, 0f);
                state.SetBelieve(Consts.CONSUMABLES, hero.Model.EquippedConsumables != null ? hero.Model.EquippedConsumables.Count : 0);
                state.SetBelieve(Consts.WEAPON_TIER, hero.Model.WeaponTier);
                state.SetBelieve(Consts.ARMOR_TIER, hero.Model.ArmorTier);
                state.SetBelieve(Consts.AMULET_TIER, hero.Model.AmuletTier);
            }

            return state;
        }

        private void CreateBelieves()
        {
        }

        private void CreateGoals()
        {
            var hasMarket = _buildings != null && _buildings.Market != null && !string.IsNullOrWhiteSpace(_buildings.Market.Id);
            var hasBlacksmith = _buildings != null && _buildings.Blacksmith != null && !string.IsNullOrWhiteSpace(_buildings.Blacksmith.Id);

            Goals.Add(CreateGoal()
                .WithName("Defend Building")
                .WithPriority(6)
                .WithImportance(ctx => ctx.state.GetBelieve(Consts.DEFEND_ACTIVE) > 0.5f ? 5f : 0f)
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.DEFEND_ACTIVE) <= 0.5f)
                .WithHeuristic(ctx => ctx.state.GetBelieve(Consts.DEFEND_ACTIVE) > 0.5f ? 1f : 0f)
                .WithIcon("Icons/all/lorc/shield")
                .Build());

            Goals.Add(CreateGoal()
                .WithName("Be Alive")
                .WithPriority(5)
                .WithImportance(ctx => Mathf.Clamp(70f - ctx.state.GetBelieve(Consts.HEALTH), 0f, 100f) / 100f)
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.HEALTH) >= 85f)
                
                .WithHeuristic(ctx =>
                {
                    if (ctx.state.GetBelieve(Consts.HEALTH) >= 85f)
                    {
                        return 0f;
                    }

                    var progress = Mathf.Clamp01(ctx.state.GetBelieve(Consts.HEALTH) / 85f);
                    return 1f - progress;
                })
                .Build()
            );

            Goals.Add(CreateGoal()
                .WithName("Defend")
                .WithPriority(4)
                .WithImportance(ctx => ctx.state.GetBelieve(Consts.ENEMIES_NEARBY) > 0.1f ? 1.5f : 0f)
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.ENEMIES_NEARBY) <= 0.1f)
                .WithHeuristic(ctx => ctx.state.GetBelieve(Consts.ENEMIES_NEARBY) > 0.1f ? 1f : 0f)
                .WithIcon("Icons/all/lorc/shield")
                .Build());

            Goals.Add(CreateGoal()
                .WithName("Get Gold")
                .WithPriority(3)
                .WithImportance(ctx =>
                {
                    var g = ctx.state.GetBelieve(Consts.GOLD);
                    if (g >= 200f)
                    {
                        return 0f;
                    }
                    return Mathf.Clamp01((200f - g) / 200f);
                })
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.GOLD) >= 200f)
                .WithHeuristic(ctx =>
                {
                    var g = ctx.state.GetBelieve(Consts.GOLD);
                    var progress = Mathf.Clamp01(g / 200f);
                    return 1f - progress;
                })
                .WithIcon("Icons/coin")
                .Build());
            
            if (hasBlacksmith)
            {
                AddTieredGoal("Weapon Tier", Consts.WEAPON_TIER, maxTier: 3, priority: 1, baseImportance: 0.2f);
                AddTieredGoal("Armor Tier", Consts.ARMOR_TIER, maxTier: 3, priority: 1, baseImportance: 0.2f);
            }

            if (hasMarket)
            {
                AddConsumableTierGoals(priority: 5, baseImportance: 1.0f);
                AddTieredGoal("Amulet Tier", Consts.AMULET_TIER, maxTier: 3, priority: 2, baseImportance: 0.2f);
            }
        }

        private void AddConsumableTierGoals(int priority, float baseImportance)
        {
            for (var t = 1; t <= DesiredConsumables; t++)
            {
                var tier = t;
                Goals.Add(CreateGoal()
                    .WithName($"Have {tier} Consumable{(tier == 1 ? string.Empty : "s")}")
                    .WithPriority(priority)
                    .WithImportance(ctx =>
                    {
                        var c = (int)ctx.state.GetBelieve(Consts.CONSUMABLES);
                        if (c >= tier)
                        {
                            return 0f;
                        }

                        if (tier != c + 1)
                        {
                            return 0f;
                        }

                        return baseImportance;
                    })
                    .WithAchieved(ctx => ctx.state.GetBelieve(Consts.CONSUMABLES) >= tier)
                    .WithHeuristic(ctx =>
                    {
                        var c = ctx.state.GetBelieve(Consts.CONSUMABLES);
                        var progress = Mathf.Clamp01(c / tier);
                        return 1f - progress;
                    })
                    .Build());
            }
        }

        private void AddTieredGoal(string label, int beliefIndex, int maxTier, int priority, float baseImportance)
        {
            for (var t = 1; t <= maxTier; t++)
            {
                var tier = t;
                Goals.Add(CreateGoal()
                    .WithName($"{label} >= {tier}")
                    .WithPriority(priority)
                    .WithImportance(ctx =>
                    {
                        var current = (int)ctx.state.GetBelieve(beliefIndex);
                        if (current >= tier)
                        {
                            return 0f;
                        }

                        if (tier != current + 1)
                        {
                            return 0f;
                        }

                        return baseImportance;
                    })
                    .WithAchieved(ctx => ctx.state.GetBelieve(beliefIndex) >= tier)
                    .WithHeuristic(ctx =>
                    {
                        var current = ctx.state.GetBelieve(beliefIndex);
                        var progress = Mathf.Clamp01(current / tier);
                        return 1f - progress;
                    })
                    .Build());
            }
        }

        private void CreateActions()
        {
            var guildId = _buildings != null && _buildings.Guild != null ? _buildings.Guild.Id : string.Empty;
            var marketId = _buildings != null && _buildings.Market != null ? _buildings.Market.Id : string.Empty;
            var blacksmithId = _buildings != null && _buildings.Blacksmith != null ? _buildings.Blacksmith.Id : string.Empty;
            var guildName = _buildings != null && _buildings.Guild != null ? _buildings.Guild.DisplayName : "Guild";
            var marketName = _buildings != null && _buildings.Market != null ? _buildings.Market.DisplayName : "Market";
            var blacksmithName = _buildings != null && _buildings.Blacksmith != null ? _buildings.Blacksmith.DisplayName : "Blacksmith";

            if (!string.IsNullOrWhiteSpace(guildId))
            {
                // Guild is currently not a functional destination; avoid it as a generic move step.
            }

            if (!string.IsNullOrWhiteSpace(marketId))
            {
                Actions.Add(MakeMoveAction($"Go to {marketName}", marketId, marketName));
                Actions.Add(MakeBuyConsumableAction(marketId, "Buy Health Potion (Market)"));
                Actions.Add(MakeBuyAmuletAction(marketId, "Buy Better Amulet (Market)"));
            }

            if (!string.IsNullOrWhiteSpace(blacksmithId))
            {
                Actions.Add(MakeMoveAction($"Go to {blacksmithName}", blacksmithId, blacksmithName));
                Actions.Add(MakeBuyWeaponAction(blacksmithId, "Buy Better Weapon"));
                Actions.Add(MakeBuyArmorAction(blacksmithId, "Buy Better Armor"));
            }

            

            Actions.Add(MakeDefendBuildingAction());
            Actions.Add(MakeUseHealingConsumableAction());
            Actions.Add(MakeAcceptBestQuestAction());
            Actions.Add(MakeDoActiveQuestAction());
            Actions.Add(MakeFightMonsterAction());
            Actions.Add(MakeHuntMonsterForGoldAction());
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeAcceptBestQuestAction()
        {
            return CreateAction()
                .WithName("Accept Quest")
                .WithIcon("Icons/all/lorc/contract")
                .WithPreCondition(ctx =>
                {
                    if (ctx.state.GetBelieve(Consts.HAS_ACTIVE_QUEST) > 0.5f)
                    {
                        return false;
                    }

                    if (ctx.state.GetBelieve(Consts.BEST_QUEST_EXISTS) <= 0.5f)
                    {
                        return false;
                    }

                    return ctx.state.GetBelieve(Consts.BEST_QUEST_SCORE) >= 0.75f;
                })
                .WithPreConditionsDescription("Quest available")
                .WithTime(_ => 0.1f)
                .WithEffect(ctx => ctx.state.Clone().Mutate((ref AgentState s) =>
                {
                    s.SetBelieve(Consts.HAS_ACTIVE_QUEST, 1f);
                    s.SetBelieve(Consts.ACTIVE_QUEST_TARGET_X, s.GetBelieve(Consts.BEST_QUEST_TARGET_X));
                    s.SetBelieve(Consts.ACTIVE_QUEST_TARGET_Z, s.GetBelieve(Consts.BEST_QUEST_TARGET_Z));
                }))
                .WithEffectDescription("Accept quest")
                .WithImplementation((agent, ctx) => new Strategies.QuestAcceptStrategy(agent, QuestRuntimeConfig.Service))
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeDoActiveQuestAction()
        {
            return CreateAction()
                .WithName("Do Quest")
                .WithIcon("Icons/all/lorc/crossed-swords")
                .WithPreCondition(ctx => ctx.state.GetBelieve(Consts.HAS_ACTIVE_QUEST) > 0.5f)
                .WithPreConditionsDescription("Have active quest")
                .WithTime(_ => 1.0f)
                .WithEffect(ctx => ctx.state.Clone().Mutate((ref AgentState s) =>
                {
                    s.SetBelieve(Consts.GOLD, s.GetBelieve(Consts.GOLD) + Mathf.Max(0f, s.GetBelieve(Consts.BEST_QUEST_SHARE)));
                }))
                .WithEffectDescription("Complete quest")
                .WithImplementation((agent, ctx) =>
                {
                    var hero = agent != null ? agent.GetComponent<HeroFacade>() : null;
                    if (hero?.Model == null || QuestRuntimeConfig.Service == null)
                    {
                        return null;
                    }

                    if (!QuestRuntimeConfig.Service.TryGetById(hero.Model.ActiveQuestId, out var q) || q == null)
                    {
                        return null;
                    }

                    if (q.TargetKind == QuestTargetKind.Building)
                    {
                        var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(x => x != null && x.Id == q.TargetInstanceId));
                        return building != null ? new Strategies.AttackBuildingStrategy(agent, ctx, building) : null;
                    }

                    var monster = Registry<MonsterFacade>.Get(items => items.FirstOrDefault(x => x != null && x.InstanceId == q.TargetInstanceId));
                    return monster != null ? new Strategies.FightMonsterStrategy(agent, ctx, monster, null) : null;
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeUseHealingConsumableAction()
        {
            return CreateAction()
                .WithName("Use Health Potion")
                .WithIcon("Icons/all/lorc/standing-potion")
                .WithPreCondition(ctx =>
                {
                    if (ctx.state.GetBelieve(Consts.CONSUMABLES) <= 0.1f)
                    {
                        return false;
                    }

                    return ctx.state.GetBelieve(Consts.HEALTH_PCT) <= 0.40f;
                })
                .WithPreConditionsDescription("Have potion and HP < 40%")
                .WithTime(_ => 0.1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.CONSUMABLES, Mathf.Max(0f, s.GetBelieve(Consts.CONSUMABLES) - 1f));
                        s.SetBelieve(Consts.HEALTH_PCT, Mathf.Max(s.GetBelieve(Consts.HEALTH_PCT), 0.75f));
                    }))
                .WithEffectDescription("Heal")
                .WithImplementation((agent, ctx) => new UseHealingConsumableStrategy(agent))
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeDefendBuildingAction()
        {
            return CreateAction()
                .WithName("Defend Building")
                .WithIcon("Icons/all/lorc/shield")
                .WithPreCondition(ctx =>
                {
                    return ctx.state.GetBelieve(Consts.DEFEND_ACTIVE) > 0.5f;
                })
                .WithPreConditionsDescription("Have building defense target")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx => ctx.state.Clone().Mutate((ref AgentState s) =>
                {
                    var x = ctx.state.GetBelieve(Consts.DEFEND_X);
                    var z = ctx.state.GetBelieve(Consts.DEFEND_Z);
                    s.SetLocation(x, z);
                }))
                .WithEffectDescription("Move to attacked building")
                .WithImplementation((agent, ctx) =>
                {
                    if (_hero?.Model == null)
                    {
                        return null;
                    }

                    var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(b => b != null && b.Id == _hero.Model.DefendBuildingInstanceId));
                    if (building == null)
                    {
                        return null;
                    }

                    return new DefendBuildingStrategy(agent, ctx, building);
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeFightMonsterAction()
        {
            return CreateAction()
                .WithName("Fight Monster")
                .WithIcon("Icons/all/lorc/crossed-swords")
                .WithPreCondition(ctx =>
                {
                    return ctx.state.GetBelieve(Consts.ENEMIES_NEARBY) > 0.1f;
                })
                .WithPreConditionsDescription("Monster nearby")
                .WithTime(_ => 1.0f)
                .WithEffect(ctx => ctx.state)
                .WithEffectDescription("Fight monster")
                .WithImplementation((agent, ctx) =>
                {
                    if (!TryFindNearestMonster(out var monster))
                    {
                        return null;
                    }

                    return new FightMonsterStrategy(agent, ctx, monster, null);
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeHuntMonsterForGoldAction()
        {
            return CreateAction()
                .WithName("Hunt Monster")
                .WithIcon("Icons/all/lorc/crossed-swords")
                .WithPreCondition(ctx =>
                {
                    if (ctx.state.GetBelieve(Consts.DEFEND_ACTIVE) > 0.5f)
                    {
                        return false;
                    }

                    return ctx.state.GetBelieve(Consts.ENEMIES_NEARBY) > 0.1f;
                })
                .WithPreConditionsDescription("Monster exists")
                .WithTime(_ => 1.0f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.GOLD, s.GetBelieve(Consts.GOLD) + HuntExpectedGold);
                    }))
                .WithEffectDescription("Earn gold from monster")
                .WithImplementation((agent, ctx) =>
                {
                    if (!TryFindNearestMonster(out var monster))
                    {
                        return null;
                    }

                    return new FightMonsterStrategy(agent, ctx, monster, null);
                })
                .Build();
        }

        private bool TryFindNearestMonster(out MonsterFacade monster)
        {
            monster = null;
            if (_hero == null || _hero.Model == null)
            {
                return false;
            }

            var sensor = _hero != null ? _hero.EnemySensor : null;
            if (sensor == null)
            {
                return false;
            }

            if (!sensor.TryGetNearestEnemy(_hero.transform.position, out var t))
            {
                return false;
            }

            monster = t != null ? t.GetComponentInParent<MonsterFacade>() : null;
            return monster != null && monster.IsAlive;
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeBuyConsumableAction(string buildingDefinitionId, string actionName)
        {
            return CreateAction()
                .WithName(actionName)
                .WithIcon("Icons/all/lorc/standing-potion")
                .WithPreCondition(ctx =>
                {
                    if (GetConsumableCount() >= DesiredConsumables)
                    {
                        return false;
                    }

                    if (ctx.state.GetBelieve(Consts.CONSUMABLES) >= DesiredConsumables)
                    {
                        return false;
                    }

                    
                    if (!ctx.world.Locations.HasAny(buildingDefinitionId) || !IsAt(ctx, buildingDefinitionId))
                    {
                        DebugGoap(ctx, $"{actionName}: not at shop", buildingDefinitionId);
                        return false;
                    }

                    
                    var ok = TryPickBestBuyCandidate(ctx, buildingDefinitionId, IsConsumableCandidate, out _, out _, out _);
                    if (!ok)
                    {
                        DebugGoap(ctx, $"{actionName}: no candidate (locked/too expensive/missing)", buildingDefinitionId);
                    }
                    return ok;
                })
                .WithPreConditionsDescription("At shop and item available")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                {
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId, IsConsumableCandidate, out _, out var item, out var cost))
                    {
                        return ctx.state;
                    }

                    return ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.GOLD, Mathf.Max(0f, s.GetBelieve(Consts.GOLD) - cost));
                        s.SetBelieve(Consts.CONSUMABLES, s.GetBelieve(Consts.CONSUMABLES) + 1f);
                    });
                })
                .WithEffectDescription("Buy consumable")
                .WithImplementation((agent, ctx) =>
                {
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId, IsConsumableCandidate, out var building, out var item, out _))
                    {
                        return null;
                    }

                    return new ShopPurchaseStrategy(agent, ctx, building, item);
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeBuyWeaponAction(string buildingDefinitionId, string actionName)
        {
            return CreateAction()
                .WithName(actionName)
                .WithIcon("Icons/all/lorc/sword-smithing")
                .WithPreCondition(ctx =>
                {
                    if (!ctx.world.Locations.HasAny(buildingDefinitionId) || !IsAt(ctx, buildingDefinitionId))
                    {
                        DebugGoap(ctx, $"{actionName}: not at shop", buildingDefinitionId);
                        return false;
                    }

                    var currentTier = (int)ctx.state.GetBelieve(Consts.WEAPON_TIER);
                    var ok = TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                        item => item != null && item.Slot == EquipmentSlot.Weapon && item.Tier > currentTier,
                        out _, out _, out _);
                    if (!ok)
                    {
                        DebugGoap(ctx, $"{actionName}: no candidate (locked/too expensive/not an upgrade)", buildingDefinitionId);
                    }
                    return ok;
                })
                .WithPreConditionsDescription("At blacksmith and weapon available")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.WEAPON_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            item => item != null && item.Slot == EquipmentSlot.Weapon && item.Tier > currentTier,
                            out _, out var itemDef, out var cost))
                    {
                        return ctx.state;
                    }

                    return ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.GOLD, Mathf.Max(0f, s.GetBelieve(Consts.GOLD) - cost));
                        s.SetBelieve(Consts.WEAPON_TIER, Mathf.Max(s.GetBelieve(Consts.WEAPON_TIER), itemDef != null ? itemDef.Tier : 0));
                    });
                })
                .WithEffectDescription("Buy better weapon")
                .WithImplementation((agent, ctx) =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.WEAPON_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            i => i != null && i.Slot == EquipmentSlot.Weapon && i.Tier > currentTier,
                            out var building, out var item, out _))
                    {
                        return null;
                    }

                    return new ShopPurchaseStrategy(agent, ctx, building, item);
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeBuyArmorAction(string buildingDefinitionId, string actionName)
        {
            return CreateAction()
                .WithName(actionName)
                .WithIcon("Icons/all/lorc/armor-vest")
                .WithPreCondition(ctx =>
                {
                    if (!ctx.world.Locations.HasAny(buildingDefinitionId) || !IsAt(ctx, buildingDefinitionId))
                    {
                        DebugGoap(ctx, $"{actionName}: not at shop", buildingDefinitionId);
                        return false;
                    }

                    var currentTier = (int)ctx.state.GetBelieve(Consts.ARMOR_TIER);
                    var ok = TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                        item => item != null && item.Slot == EquipmentSlot.Armor && item.Tier > currentTier,
                        out _, out _, out _);
                    if (!ok)
                    {
                        DebugGoap(ctx, $"{actionName}: no candidate (locked/too expensive/not an upgrade)", buildingDefinitionId);
                    }
                    return ok;
                })
                .WithPreConditionsDescription("At blacksmith and armor available")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.ARMOR_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            item => item != null && item.Slot == EquipmentSlot.Armor && item.Tier > currentTier,
                            out _, out var itemDef, out var cost))
                    {
                        return ctx.state;
                    }

                    return ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.GOLD, Mathf.Max(0f, s.GetBelieve(Consts.GOLD) - cost));
                        s.SetBelieve(Consts.ARMOR_TIER, Mathf.Max(s.GetBelieve(Consts.ARMOR_TIER), itemDef != null ? itemDef.Tier : 0));
                    });
                })
                .WithEffectDescription("Buy better armor")
                .WithImplementation((agent, ctx) =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.ARMOR_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            i => i != null && i.Slot == EquipmentSlot.Armor && i.Tier > currentTier,
                            out var building, out var item, out _))
                    {
                        return null;
                    }

                    return new ShopPurchaseStrategy(agent, ctx, building, item);
                })
                .Build();
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeBuyAmuletAction(string buildingDefinitionId, string actionName)
        {
            return CreateAction()
                .WithName(actionName)
                .WithIcon("Icons/all/lorc/ankh")
                .WithPreCondition(ctx =>
                {
                    if (!ctx.world.Locations.HasAny(buildingDefinitionId) || !IsAt(ctx, buildingDefinitionId))
                    {
                        DebugGoap(ctx, $"{actionName}: not at shop", buildingDefinitionId);
                        return false;
                    }

                    var currentTier = (int)ctx.state.GetBelieve(Consts.AMULET_TIER);
                    var ok = TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                        item => item != null && item.Slot == EquipmentSlot.Item && !item.IsSingleUse && item.Tier > currentTier,
                        out _, out _, out _);
                    if (!ok)
                    {
                        DebugGoap(ctx, $"{actionName}: no candidate (locked/too expensive)", buildingDefinitionId);
                    }
                    return ok;
                })
                .WithPreConditionsDescription("At market and amulet available")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.AMULET_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            item => item != null && item.Slot == EquipmentSlot.Item && !item.IsSingleUse && item.Tier > currentTier,
                            out _, out var itemDef, out var cost))
                    {
                        return ctx.state;
                    }

                    return ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(Consts.GOLD, Mathf.Max(0f, s.GetBelieve(Consts.GOLD) - cost));
                        s.SetBelieve(Consts.AMULET_TIER, Mathf.Max(s.GetBelieve(Consts.AMULET_TIER), itemDef != null ? itemDef.Tier : 0));
                    });
                })
                .WithEffectDescription("Buy better amulet")
                .WithImplementation((agent, ctx) =>
                {
                    var currentTier = (int)ctx.state.GetBelieve(Consts.AMULET_TIER);
                    if (!TryPickBestBuyCandidate(ctx, buildingDefinitionId,
                            i => i != null && i.Slot == EquipmentSlot.Item && !i.IsSingleUse && i.Tier > currentTier,
                            out var building, out var item, out _))
                    {
                        return null;
                    }

                    return new ShopPurchaseStrategy(agent, ctx, building, item);
                })
                .Build();
        }

        private static bool TryPickBestBuyCandidate(
            AgentContext<GameWorldSnapshot> ctx,
            string buildingDefinitionId,
            System.Func<ItemDefinition, bool> predicate,
            out Buildings.BuildingFacade building,
            out Content.Heroes.ItemDefinition item,
            out float cost)
        {
            building = null;
            item = null;
            cost = 0f;

            var gold = ctx.state.GetBelieve(Consts.GOLD);
            if (gold <= 0.1f)
            {
                return false;
            }

            
            if (!ctx.world.Locations.TryGetClosestLocation(buildingDefinitionId, ctx.state.Location, out var shopLoc))
            {
                return false;
            }

            
            foreach (var b in Registry.Registry<Buildings.BuildingFacade>.All())
            {
                if (b == null || !b.IsAlive || b.Definition == null || b.Model == null)
                {
                    continue;
                }

                if (b.Id == shopLoc.ID)
                {
                    building = b;
                    break;
                }
            }

            
            if (building == null)
            {
                foreach (var b in Registry.Registry<Buildings.BuildingFacade>.All())
                {
                    if (b == null || !b.IsAlive || b.Definition == null || b.Model == null)
                    {
                        continue;
                    }

                    if (b.Definition.Id == buildingDefinitionId)
                    {
                        building = b;
                        break;
                    }
                }
            }

            if (building == null || building.Definition.SellItems == null)
            {
                return false;
            }

            
            if (building.Definition.SellItems.Length == 0)
            {
                return false;
            }

            
            float bestScore = float.MinValue;
            foreach (var candidate in building.Definition.SellItems)
            {
                if (candidate == null || string.IsNullOrWhiteSpace(candidate.Id))
                {
                    continue;
                }

                
                
                

                if (predicate != null && !predicate(candidate))
                {
                    continue;
                }

                if (!building.Model.IsSellItemUnlocked(candidate.Id))
                {
                    continue;
                }

                var c = candidate.GoldCost;
                if (c <= 0 || gold < c)
                {
                    continue;
                }

                var score = candidate.Attack + candidate.Defense + candidate.Speed;
                if (score <= bestScore)
                {
                    continue;
                }

                bestScore = score;
                item = candidate;
                cost = c;
            }

            return item != null;
        }

        private int GetConsumableCount()
        {
            return _hero?.Model?.EquippedConsumables != null ? _hero.Model.EquippedConsumables.Count : 0;
        }

        private void DebugGoap(AgentContext<GameWorldSnapshot> ctx, string reason, string buildingDefinitionId = null)
        {
            _ = ctx;
            _ = reason;
            _ = buildingDefinitionId;
        }

        private bool IsConsumableCandidate(ItemDefinition item)
        {
            return item != null && item.IsSingleUse;
        }

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeMoveAction(string name, string locationId, string locationLabel)
        {
            return CreateAction()
                .WithName(name)
                .WithIcon("Icons/all/lorc/treasure-map")
                .WithPreCondition(ctx =>
                {
                    var ok = !IsAt(ctx, locationId) && ctx.world.Locations.HasAny(locationId);
                    if (!ok)
                    {
                        DebugGoap(ctx, $"{name}: move precondition failed", locationId);
                    }
                    return ok;
                })
                .WithPreConditionsDescription($"Not at {locationLabel}")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        // Use the closest known location marker, not the current simulated position.
                        if (ctx.world.Locations.TryGetClosest(locationId, ctx.state.Location, out var pos))
                        {
                            s.SetLocation(pos);
                        }
                    })
                )
                .WithEffectDescription($"Move to {locationLabel}")
                .WithImplementation((agent, ctx) =>
                {
                    if (!ctx.world.Locations.TryGetClosest(locationId, ctx.state.Location, out var dest2d))
                    {
                        Debug.LogError($"Could not find location {locationLabel} ({locationId})");
                        return null;
                    }
                    
                    var destination = new Vector3(dest2d.x, agent.transform.position.y, dest2d.y);
                    return new MoveStrategy<GameWorldSnapshot, HeroAnimationController>(destination, agent, ctx);
                })
                .Build();
        }

        private void CreateWanderIdleAction()
        {
            IdleActions.Add(new IdleAction<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>(
                "Wander",
                "Idle or wander when no plan",
                (agent, ctx) => new WanderStrategy<GameWorldSnapshot, HeroAnimationController>(agent, ctx, WanderRadius)
            ));
        }
        
        private bool IsAt(AgentContext<GameWorldSnapshot> ctx, string locationId)
        {
            if (!ctx.world.Locations.TryGetClosestLocation(locationId, ctx.state.Location, out var loc))
            {
                return false;
            }

            
            var threshold = Mathf.Clamp(loc.Radius + 0.75f, 1.5f, 20f);
            return Vector2.Distance(ctx.state.Location, loc.Position) <= threshold;
        }
    }
}


