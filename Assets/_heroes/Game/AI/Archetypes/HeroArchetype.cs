using System.Collections.Generic;
using Heroes.Game.Heroes;
using Heroes.Game.AI.Strategies;
using Heroes.GOAP.Core;
using Heroes.GOAP;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class HeroArchetype :  Archetype<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>
    {
        private const float WanderRadius = 6f;
        private const float MaxGearLevel = 3f;
        private readonly string _homeBuildingInstanceId;

        public HeroArchetype(HeroFacade hero) : base(
            new List<Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot>>(),
            new List<Goal<GameWorldSnapshot>>(),
            CreateBaseState(hero))
        {
            _homeBuildingInstanceId = hero?.Model?.HomeBuildingInstanceId ?? string.Empty;
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
                state.SetBelieve(Consts.GEAR_LEVEL, hero.Model.GearLevel);
                state.SetBelieve(Consts.DANGER_LEVEL, hero.Model.DangerLevel);
            }

            return state;
        }

        private void CreateBelieves()
        {
        }

        private void CreateGoals()
        {
            Goals.Add(CreateGoal()
                .WithName("Be Alive")
                .WithPriority(5)
                .WithImportance(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) * Mathf.Clamp(70f - ctx.state.GetBelieve(Consts.HEALTH), 0f, 100f))
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) <= 0.01f || ctx.state.GetBelieve(Consts.HEALTH) >= 85f)
                .WithHeuristic(ctx => (1f - ctx.state.GetBelieve(Consts.DANGER_LEVEL) + ctx.state.GetBelieve(Consts.HEALTH) / 85f) / 2f)
                .Build()
            );
            
            Goals.Add(CreateGoal()
                .WithName("Have Best Gear")
                .WithPriority(1)
                .WithImportance(ctx => 1f)
                .WithAchieved(ctx => ctx.state.GetBelieve(Consts.GEAR_LEVEL) >= MaxGearLevel)
                .WithHeuristic(ctx => ctx.state.GetBelieve(Consts.GEAR_LEVEL) / MaxGearLevel)
                .Build()
            );
        }

        private void CreateActions()
        {
            Actions.Add(MakeMoveAction("Go to Barracks", Consts.Locations.Barracks));
            Actions.Add(MakeMoveHomeAction());
        }
        
        private Action<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> MakeMoveAction(string name, string locationId)
        {
            return CreateAction()
                .WithName(name)
                .WithPreCondition(ctx => !IsAt(ctx, locationId) && ctx.world.Locations.HasAny(locationId))
                .WithPreConditionsDescription($"Not at {locationId}")
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        if (ctx.world.Locations.TryGetClosest(locationId, ctx.state.Location, out var pos))
                        {
                            s.SetLocation(pos);
                        }
                    })
                )
                .WithEffectDescription($"Move to {locationId}")
                .WithImplementation((agent, ctx) =>
                {
                    if (!ctx.world.Locations.TryGetClosest(locationId, ctx.state.Location, out var dest2d))
                    {
                        Debug.LogError($"Could not find location {locationId}");
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
                .WithPreCondition(ctx => ctx.state.GetBelieve(Consts.DANGER_LEVEL) > 0.2f && ctx.world.Locations.TryGetPositionByInstanceId(_homeBuildingInstanceId, out _))
                .WithPreConditionsDescription("Danger above 0.2 and home exists")
                .WithTime(_ => 1f)
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
            return ctx.world.Locations.TryGetClosest(locationId, ctx.state.Location, out var pos) && Vector2.Distance(ctx.state.Location, pos) <= 2f;
        }
    }
}
