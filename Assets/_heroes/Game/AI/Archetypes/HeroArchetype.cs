using System.Collections.Generic;
using Heroes.Game.Heroes;
using Heroes.Game.AI.Strategies;
using Heroes.GOAP.Core;
using Heroes.GOAP;
using UnityEngine;
using Heroes.Content.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Buildings;
using Registry;

namespace Heroes.Game.AI
{
        public class HeroArchetype :  Archetype<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>
        {
            private const float WanderRadius = 6f;
            private const int DesiredConsumables = 3;
        private readonly string _homeBuildingInstanceId;
        private readonly GoapBuildingReferences _buildings;
        private readonly HeroFacade _hero;
        private float _nextDebugLogAt;

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
                state.SetBelieve(Consts.DANGER_LEVEL, hero.Model.DangerLevel);
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
                .WithName("Be Alive")
                .WithPriority(5)
                .WithImportance(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) * Mathf.Clamp(70f - ctx.state.GetBelieve(Consts.HEALTH), 0f, 100f))
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) <= 0.01f || ctx.state.GetBelieve(Consts.HEALTH) >= 85f)
                
                .WithHeuristic(ctx =>
                {
                    if (ctx.state.GetBelieve(Consts.DANGER_LEVEL) <= 0.01f || ctx.state.GetBelieve(Consts.HEALTH) >= 85f)
                    {
                        return 0f;
                    }

                    var progress = (1f - ctx.state.GetBelieve(Consts.DANGER_LEVEL) + ctx.state.GetBelieve(Consts.HEALTH) / 85f) / 2f;
                    return Mathf.Clamp01(1f - progress);
                })
                .Build()
            );
            
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
                Actions.Add(MakeMoveAction($"Go to {guildName}", guildId, guildName));
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

            Actions.Add(MakeMoveHomeAction());
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
            if (_buildings == null || !_buildings.EnableGoapDebugLogs)
            {
                return;
            }

            var now = Time.unscaledTime;
            
            if (now < _nextDebugLogAt)
            {
                return;
            }

            _nextDebugLogAt = now + 1.0f;

            var gold = ctx.state.GetBelieve(Consts.GOLD);
            var weaponTier = ctx.state.GetBelieve(Consts.WEAPON_TIER);
            var armorTier = ctx.state.GetBelieve(Consts.ARMOR_TIER);
            var amuletTier = ctx.state.GetBelieve(Consts.AMULET_TIER);
            var loc = ctx.state.Location;
            var weapon = _hero?.Model != null ? _hero.Model.EquippedWeaponId : string.Empty;
            var armor = _hero?.Model != null ? _hero.Model.EquippedArmorId : string.Empty;

            var hasAny = !string.IsNullOrWhiteSpace(buildingDefinitionId) && ctx.world.Locations.HasAny(buildingDefinitionId);
            var isAt = !string.IsNullOrWhiteSpace(buildingDefinitionId) && IsAt(ctx, buildingDefinitionId);
            var closest = "-";
            if (!string.IsNullOrWhiteSpace(buildingDefinitionId) && ctx.world.Locations.TryGetClosestLocation(buildingDefinitionId, ctx.state.Location, out var l))
            {
                closest = $"id={l.ID} pos={l.Position} r={l.Radius:0.##}";
            }

            UnityEngine.Debug.Log($"[GOAP] {(_hero != null ? _hero.Name : "<hero>")} {reason} gold={gold:0} tiers(w={weaponTier:0} a={armorTier:0} am={amuletTier:0}) loc={loc} weaponId={weapon} armorId={armor} hasAny={hasAny} isAt={isAt} closest=({closest}) defId={buildingDefinitionId}");
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

        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeMoveHomeAction()
        {
            return CreateAction()
                .WithName("Go Home")
                .WithIcon("Icons/all/lorc/treasure-map")
                .WithPreCondition(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) > 0.2f && ctx.world.Locations.TryGetPositionByInstanceId(_homeBuildingInstanceId, out _))
                .WithPreConditionsDescription("Danger above 0.2 and home exists")
                .WithTime(_ => 0.5f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        if (ctx.world.Locations.TryGetPositionByInstanceId(_homeBuildingInstanceId, out var pos))
                        {
                            s.SetLocation(pos);
                            s.SetBelieve(Consts.DANGER_LEVEL, 0f);
                        }
                    }))
                .WithEffectDescription("Move to home")
                .WithImplementation((agent, ctx) =>
                {
                    if (!ctx.world.Locations.TryGetPositionByInstanceId(_homeBuildingInstanceId, out var dest2d))
                    {
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


