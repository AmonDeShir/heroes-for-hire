using NUnit.Framework;

namespace Heroes.GOAP.Core.Tests
{
    public class GoalTests
    {
        [Test]
        public void Builder_Defaults_AreSet()
        {
            var goal = new Goal.Builder().Build();
            var ctx = new AgentContext(new AgentState(1));

            Assert.AreEqual(1, goal.Priority);
            Assert.AreEqual(string.Empty, goal.Name);
            Assert.AreEqual(string.Empty, goal.Description);
            Assert.AreEqual(0f, goal.Importance(ctx));
            Assert.IsFalse(goal.IsAchieved(ctx));
            Assert.AreEqual(1f, goal.Heuristic(ctx));
        }

        [Test]
        public void Execute_MultipliesImportanceByPriority()
        {
            var goal = new Goal.Builder()
                .WithPriority(3)
                .WithImportance(_ => 2f)
                .Build();

            var result = goal.Execute(new AgentContext(new AgentState(1)));

            Assert.AreEqual(6f, result);
        }

        [Test]
        public void Builder_AssignsDelegates()
        {
            var goal = new Goal.Builder()
                .WithName("FindFood")
                .WithDescription("Reach full")
                .WithAchieved(ctx => ctx.state.GetBelieve(0) >= 1f)
                .WithHeuristic(ctx => 1f - ctx.state.GetBelieve(0))
                .Build();

            var state = new AgentState(1);
            state.SetBelieve(0, 0.5f);
            var ctx = new AgentContext(state);

            Assert.AreEqual("FindFood", goal.Name);
            Assert.AreEqual("Reach full", goal.Description);
            Assert.IsFalse(goal.IsAchieved(ctx));
            Assert.AreEqual(0.5f, goal.Heuristic(ctx));

            state.SetBelieve(0, 1f);
            ctx = new AgentContext(state);
            Assert.IsTrue(goal.IsAchieved(ctx));
        }
    }
}
