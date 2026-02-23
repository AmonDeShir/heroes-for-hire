using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Heroes.GOAP;

namespace Heroes.GOAP.Core.Tests
{

    public static class Beliefs
    {
        public const int Gold = 0;
        public const int HasPickaxe = 1;
        public const int HasSword = 2;
    }

    public class PlannerTests
    {
        private static AgentContext MakeStartContext(int beliefCount)
        {
            var s = new AgentState(beliefCount);

            s.SetBelieve(Beliefs.Gold, 0f);
            s.SetBelieve(Beliefs.HasPickaxe, 0f);
            s.SetBelieve(Beliefs.HasSword, 0f);

            return new AgentContext(s);
        }

        private static float GoldOf(AgentContext ctx) => ctx.state.GetBelieve(Beliefs.Gold);
        private static bool HasPickaxeOf(AgentContext ctx) => ctx.state.GetBelieve(Beliefs.HasPickaxe) >= 0.5f;
        private static bool HasSwordOf(AgentContext ctx) => ctx.state.GetBelieve(Beliefs.HasSword) >= 0.5f;

        private static Action Work()
        {
            return new Action.Builder()
                .WithName("Work")
                .WithPreCondition(_ => true)
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(Beliefs.Gold, s.GetBelieve(Beliefs.Gold) + 5f);
                        })
                        .Bucket(Beliefs.Gold, 5f)
                        .Clamp(Beliefs.Gold, 300f)
                )
                .Build();
        }

        private static Action BuyPickaxe()
        {
            return new Action.Builder()
                .WithName("BuyPickaxe")
                .WithPreCondition(ctx =>
                {
                    var gold = GoldOf(ctx);
                    var hasPickaxe = HasPickaxeOf(ctx);
                    return !hasPickaxe && gold >= 100f;
                })
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(Beliefs.Gold, s.GetBelieve(Beliefs.Gold) - 100f);
                            s.SetBelieve(Beliefs.HasPickaxe, 1f);
                        })
                        .Bucket(Beliefs.Gold, 5f)
                )
                .Build();
        }

        private static Action Mine()
        {
            return new Action.Builder()
                .WithName("Mine")
                .WithPreCondition(ctx => HasPickaxeOf(ctx))
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(Beliefs.Gold, s.GetBelieve(Beliefs.Gold) + 30f);
                        })
                        .Bucket(Beliefs.Gold, 5f)
                        .Clamp(Beliefs.Gold, 300f)
                )
                .Build();
        }

        private static Action BuySword()
        {
            return new Action.Builder()
                .WithName("BuySword")
                .WithPreCondition(ctx =>
                {
                    var gold = GoldOf(ctx);
                    var hasSword = HasSwordOf(ctx);
                    return !hasSword && gold >= 300f;
                })
                .WithTime(_ => 1f)
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                        {
                            s.SetBelieve(Beliefs.Gold, s.GetBelieve(Beliefs.Gold) - 300f);
                            s.SetBelieve(Beliefs.HasSword, 1f);
                        })
                        .Bucket(Beliefs.Gold, 5f)
                )
                .Build();
        }

        private static Goal HaveSwordGoal()
        {
            return new Goal.Builder()
                .WithName("HaveSword")
                .WithPriority(1)
                .WithImportance(_ => 1f)
                .WithAchieved(ctx => HasSwordOf(ctx))
                .WithHeuristic(ctx =>
                {
                    if (HasSwordOf(ctx))
                    {
                        return 0f;
                    }

                    var gold = GoldOf(ctx);
                    var missing = Mathf.Max(0f, 300f - gold);

                    return missing / 300f;
                })
                .Build();
        }

        private static AgentState ApplyPlan(AgentContext start, List<Action> plan)
        {
            var current = start.state.Clone();

            foreach (var a in plan)
            {
                var ctx = new AgentContext(current);
                Assert.IsTrue(a.PreConditions(ctx),
                    $"Action '{a.Name}' precondition failed during execution simulation.");

                current = a.Effect(ctx);
            }

            return current;
        }

        [Test]
        public void Planner_Chooses_Pickaxe_Path_As_More_Optimal()
        {
            var beliefCount = 3;
            var ctx = MakeStartContext(beliefCount);

            var actions = new List<Action>
            {
                Work(),
                BuyPickaxe(),
                Mine(),
                BuySword(),
            };

            var goals = new List<Goal>
            {
                HaveSwordGoal()
            };

            var planner = new Planner();

            var goal = planner.FindGoal(goals, ctx);
            var plan = planner.Plan(actions, goal, ctx, maxDepth: 100);

            Assert.IsNotNull(plan);
            Assert.Greater(plan.Count, 0, "Planner returned empty plan.");
            Assert.AreEqual(32,
                plan.Count,
                "Expected optimal plan length: 20 Work + BuyPickaxe + 10 Mine + BuySword = 32.");

            for (var i = 0; i < 20; i++)
            {
                Assert.AreEqual("Work", plan[i].Name, $"Expected Work at step {i}.");
            }

            Assert.AreEqual("BuyPickaxe", plan[20].Name);

            for (var i = 21; i <= 30; i++)
            {
                Assert.AreEqual("Mine", plan[i].Name, $"Expected Mine at step {i}.");
            }

            Assert.AreEqual("BuySword", plan[31].Name);

            var finalState = ApplyPlan(ctx, plan);
            var finalCtx = new AgentContext(finalState);

            Assert.IsTrue(HasSwordOf(finalCtx), "Goal should be achieved after executing the plan.");
            Assert.IsTrue(HasPickaxeOf(finalCtx), "Pickaxe should be owned in the chosen path.");
            Assert.AreEqual(0f,
                finalCtx.state.GetBelieve(Beliefs.Gold),
                0.0001f,
                "Gold should end at 0 after buying the sword.");
        }

        [Test]
        public void Planner_Returns_Empty_When_Goal_Unreachable()
        {
            var beliefCount = 3;
            var ctx = MakeStartContext(beliefCount);

            var actions = new List<Action>
            {
                Work(),
                BuyPickaxe(),
                Mine(),
            };

            var goals = new List<Goal>
            {
                HaveSwordGoal()
            };

            var planner = new Planner();
            var goal = planner.FindGoal(goals, ctx);
            var plan = planner.Plan(actions, goal, ctx, maxDepth: 100);

            Assert.IsNotNull(plan);
            Assert.AreEqual(0, plan.Count, "Expected empty plan when goal is unreachable.");
        }

        [Test]
        public void Planner_Never_Uses_Action_If_Preconditions_Not_Met()
        {
            var beliefCount = 3;
            var ctx = MakeStartContext(beliefCount);

            var mine = Mine();

            Assert.IsFalse(mine.PreConditions(ctx), "Mine should not be executable at the start (no pickaxe).");

            var actions = new List<Action>
            {
                Work(),
                BuyPickaxe(),
                mine,
                BuySword(),
            };

            var goals = new List<Goal>
            {
                HaveSwordGoal()
            };
            
            var planner = new Planner();
            var goal = planner.FindGoal(goals, ctx);
            var plan = planner.Plan(actions, goal, ctx, maxDepth: 100);

            Assert.Greater(plan.Count, 0);

            var idxMine = plan.FindIndex(a => a.Name == "Mine");
            var idxPickaxe = plan.FindIndex(a => a.Name == "BuyPickaxe");

            Assert.Greater(idxMine, idxPickaxe, "Mine should only appear after BuyPickaxe in the plan.");
        }
    }
}