using System.Collections.Generic;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.Systems.GOAP.Demo;
using GOAP.Demo.Strategies;
using UnityEngine;

namespace GOAP.Demo
{
    public class DemoArchetype : Archetype<Agent<DemoWorldSnapshot>, DemoWorldSnapshot>
    {
        public DemoArchetype(Vector2 homeLocation) : base(new List<Action<Agent<DemoWorldSnapshot>, DemoWorldSnapshot>>(), new List<Goal<DemoWorldSnapshot>>(), new AgentState(3))
        {
            BaseState.SetBelieve(DemoConsts.GOLD, 0f);
            BaseState.SetBelieve(DemoConsts.PICKAXE, 0f);
            BaseState.SetBelieve(DemoConsts.SWORD, 0f);
            BaseState.SetLocation(homeLocation);

            Goals.Add(CreateGoal()
                .WithName("Have Sword")
                .WithPriority(1)
                .WithImportance(_ => 1f)
                .WithAchieved(ctx => ctx.state.GetBelieve(DemoConsts.SWORD) >= 0.5f)
                .WithHeuristic(ctx =>
                {
                    if (ctx.state.GetBelieve(DemoConsts.SWORD) >= 0.5f)
                    {
                        return 0;
                    }
                    
                    var gold = ctx.state.GetBelieve(DemoConsts.GOLD);
                    var missing = Mathf.Max(0f, 300f - gold);

                    return missing / 300f;
                })
                .Build()
            );

            Actions.Add(MakeMoveAction("Go to Home", DemoConsts.HOME));
            Actions.Add(MakeMoveAction("Go to Work", DemoConsts.WORK));
            Actions.Add(MakeMoveAction("Go to Store", DemoConsts.STORE));
            Actions.Add(MakeMoveAction("Go to Mine", DemoConsts.MINE));

            Actions.Add(CreateAction()
                .WithName("Work")
                .WithPreCondition(ctx => IsAt(ctx, DemoConsts.WORK))
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(DemoConsts.GOLD, s.GetBelieve(DemoConsts.GOLD) + 5f);
                        })
                        .Bucket(DemoConsts.GOLD, 5f)
                        .Clamp(DemoConsts.GOLD, 300f)
                )
                .WithImplementation((agent, ctx) => new TimedRewardStrategy<DemoWorldSnapshot>(agent.Animator, ctx, DemoConsts.GOLD, 5f))
                .Build()
            );

            Actions.Add(CreateAction()
                .WithName("Mine")
                .WithPreCondition(ctx => IsAt(ctx, DemoConsts.MINE) && ctx.state.GetBelieve(DemoConsts.PICKAXE) >= 0.5f)
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(DemoConsts.GOLD, s.GetBelieve(DemoConsts.GOLD) + 30f);
                        })
                        .Bucket(DemoConsts.GOLD, 5f)
                        .Clamp(DemoConsts.GOLD, 300f)
                )
                .WithImplementation((agent, ctx) => new TimedRewardStrategy<DemoWorldSnapshot>(agent.Animator, ctx, DemoConsts.GOLD, 30f))
                .Build()
            );

            Actions.Add(CreateAction()
                .WithName("Buy Pickaxe")
                .WithPreCondition(ctx => IsAt(ctx, DemoConsts.STORE)
                    && ctx.state.GetBelieve(DemoConsts.PICKAXE) < 0.5f
                    && ctx.state.GetBelieve(DemoConsts.GOLD) >= 100f)
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(DemoConsts.GOLD, s.GetBelieve(DemoConsts.GOLD) - 100f);
                            s.SetBelieve(DemoConsts.PICKAXE, 1f);
                        })
                        .Bucket(DemoConsts.GOLD, 5f)
                )
                .WithImplementation((agent, ctx) => new BuyStrategy<DemoWorldSnapshot>(agent.Animator, ctx, DemoConsts.PICKAXE, 100f))
                .Build()
            );

            Actions.Add(CreateAction()
                .WithName("Buy Sword")
                .WithPreCondition(ctx => IsAt(ctx, DemoConsts.STORE)
                    && ctx.state.GetBelieve(DemoConsts.SWORD) < 0.5f
                    && ctx.state.GetBelieve(DemoConsts.GOLD) >= 300f)
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(DemoConsts.GOLD, s.GetBelieve(DemoConsts.GOLD) - 300f);
                            s.SetBelieve(DemoConsts.SWORD, 1f);
                        })
                        .Bucket(DemoConsts.GOLD, 5f)
                )
                .WithImplementation((agent, ctx) => new BuyStrategy<DemoWorldSnapshot>(agent.Animator, ctx, DemoConsts.SWORD, 300f))
                .Build()
            );
        }

        private Action<Agent<DemoWorldSnapshot>, DemoWorldSnapshot> MakeMoveAction(string name, string locationId)
        {
            return CreateAction()
                .WithName(name)
                .WithPreCondition(ctx => !IsAt(ctx, locationId))
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetLocation(ctx.world.Locations[locationId]);
                    })
                )
                .WithImplementation((agent, ctx) =>
                {
                    var dest2d = ctx.world.Locations[locationId];
                    var destination = new Vector3(dest2d.x, agent.transform.position.y, dest2d.y);
                    return new MoveStrategy<DemoWorldSnapshot>(destination, agent, ctx);
                })
                .Build();
        }

        private bool IsAt(AgentContext<DemoWorldSnapshot> ctx, string locationId)
        {
            return Vector2.Distance(ctx.state.Location, ctx.world.Locations[locationId]) <= 0.01f;
        }
    }
}
