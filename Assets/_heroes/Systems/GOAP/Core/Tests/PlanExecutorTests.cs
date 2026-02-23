using System.Collections.Generic;
using NUnit.Framework;
using GoapAction = Heroes.GOAP.Core.Action<object>;

namespace Heroes.GOAP.Core.Tests
{
    public class PlanExecutorTests
    {
        private sealed class ImmediateStrategy : IActionStrategy
        {
            public bool CanPreform { get; } = true;
            public bool Complete { get; private set; }

            public void Update(float deltaTime)
            {
                Complete = true;
            }
        }

        private static GoapAction MakeAction(string name)
        {
            return new GoapAction.Builder()
                .WithName(name)
                .WithPreCondition(_ => true)
                .WithTime(_ => 1f)
                .WithEffect(ctx => ctx.state.Clone().Mutate((ref AgentState s) => s.SetBelieve(0, s.GetBelieve(0) + 1f)))
                .WithImplementation(_ => new ImmediateStrategy())
                .Build();
        }

        private static Goal MakeGoal()
        {
            return new Goal.Builder()
                .WithName("ReachOne")
                .WithPriority(1)
                .WithImportance(_ => 1f)
                .WithAchieved(ctx => ctx.state.GetBelieve(0) >= 1f)
                .WithHeuristic(ctx => 1f - ctx.state.GetBelieve(0))
                .Build();
        }

        [Test]
        public void Update_RaisesNextStepLoaded_ForFirstStep()
        {
            var actions = new List<GoapAction> { MakeAction("Gain") };
            var goals = new List<Goal> { MakeGoal() };

            var archetype = new Archetype<object>(actions, goals, new AgentState(1));
            var executor = new PlanExecutor<object>(new object(), archetype);

            var events = 0;
            executor.OnNextStepLoaded += () => events++;

            executor.Update(0f);

            Assert.AreEqual(1, events);
        }

        [Test]
        public void Update_RaisesNextStepLoaded_ForEachStep()
        {
            var actions = new List<GoapAction> { MakeAction("Gain") };
            var goals = new List<Goal>
            {
                new Goal.Builder()
                    .WithName("ReachTwo")
                    .WithPriority(1)
                    .WithImportance(_ => 1f)
                    .WithAchieved(ctx => ctx.state.GetBelieve(0) >= 2f)
                    .WithHeuristic(ctx => 2f - ctx.state.GetBelieve(0))
                    .Build()
            };

            var archetype = new Archetype<object>(actions, goals, new AgentState(1));
            var executor = new PlanExecutor<object>(new object(), archetype);

            var events = 0;
            executor.OnNextStepLoaded += () => events++;

            executor.Update(0f);
            executor.Update(0f);

            Assert.AreEqual(2, events);
        }

        [Test]
        public void Update_DoesNotRaiseEvent_WhenNoPlanAvailable()
        {
            var archetype = new Archetype<object>(new List<GoapAction>(), new List<Goal>(), new AgentState(1));
            var executor = new PlanExecutor<object>(new object(), archetype);

            var events = 0;
            executor.OnNextStepLoaded += () => events++;

            executor.Update(0f);

            Assert.AreEqual(0, events);
        }
    }
}
