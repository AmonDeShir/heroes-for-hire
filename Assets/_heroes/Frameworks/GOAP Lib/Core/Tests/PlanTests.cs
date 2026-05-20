using System.Collections.Generic;
using NUnit.Framework;
using GoapAction = Heroes.GOAP.Core.Action<object, Heroes.GOAP.Core.Tests.TestWorldSnapshot>;
using GoapPlan = Heroes.GOAP.Core.Plan<object, Heroes.GOAP.Core.Tests.TestWorldSnapshot>;

namespace Heroes.GOAP.Core.Tests
{
    public class PlanTests
    {
        private sealed class TestStrategy : IActionStrategy
        {
            public bool Started { get; private set; }
            public bool Updated { get; private set; }
            public bool Stopped { get; private set; }

            public bool CanPerform { get; } = true;
            public bool Complete { get; private set; }

            public TestStrategy(bool complete)
            {
                Complete = complete;
            }

            public void Start()
            {
                Started = true;
            }

            public void Update(float deltaTime)
            {
                Updated = true;
            }

            public void Stop()
            {
                Stopped = true;
            }
        }

        private static GoapAction MakeAction(string name, bool preconditions, TestStrategy strategy)
        {
            return new GoapAction.Builder()
                .WithName(name)
                .WithPreCondition(_ => preconditions)
                .WithEffect(ctx => ctx.state.Clone())
                .WithImplementation((_, __) => strategy)
                .Build();
        }

        [Test]
        public void StartNextStep_ReturnsFalse_WhenNoSteps()
        {
            var plan = new GoapPlan(new Goal<TestWorldSnapshot>.Builder().Build(), new Stack<GoapAction>());

            var started = plan.StartNextStep(new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot()), new object());

            Assert.IsFalse(started);
            Assert.IsTrue(plan.IsEmpty);
            Assert.IsNull(plan.Step);
        }

        [Test]
        public void StartNextStep_Fails_WhenPreconditionsNotMet()
        {
            var goal = new Goal<TestWorldSnapshot>.Builder().WithName("TestGoal").Build();
            var strategy = new TestStrategy(complete: true);
            var action = MakeAction("Blocked", false, strategy);

            var stack = new Stack<GoapAction>();
            stack.Push(action);

            var plan = new GoapPlan(goal, stack);

            var started = plan.StartNextStep(new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot()), new object());

            Assert.IsFalse(started);
            Assert.IsNull(plan.Goal);
            Assert.IsNull(plan.Step);
        }

        [Test]
        public void StartNextStep_UsesStackOrder()
        {
            var strategyA = new TestStrategy(complete: true);
            var strategyB = new TestStrategy(complete: true);

            var actionA = MakeAction("A", true, strategyA);
            var actionB = MakeAction("B", true, strategyB);

            var stack = new Stack<GoapAction>();
            stack.Push(actionB);
            stack.Push(actionA);

            var plan = new GoapPlan(new Goal<TestWorldSnapshot>.Builder().Build(), stack);
            var ctx = new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot());

            Assert.IsTrue(plan.StartNextStep(ctx, new object()));
            Assert.AreEqual("A", plan.Step.Name);
            Assert.IsTrue(strategyA.Started);

            plan.Update(0f);
            Assert.IsTrue(strategyA.Updated);
            Assert.IsTrue(strategyA.Stopped);
            Assert.IsNull(plan.Step);

            Assert.IsTrue(plan.StartNextStep(ctx, new object()));
            Assert.AreEqual("B", plan.Step.Name);
            Assert.IsTrue(strategyB.Started);
        }
    }
}


