using NUnit.Framework;
using GoapAction = Heroes.GOAP.Core.Action<object, Heroes.GOAP.Core.Tests.TestWorldSnapshot>;

namespace Heroes.GOAP.Core.Tests
{
    public class StrategyHandler : IActionStrategy
    {
        private readonly System.Action impl;

        public StrategyHandler(System.Action impl)
        {
            this.impl = impl;
        }

        public bool CanPerform { get; } = true;
        public bool Complete { get; private set; } = false;

        public void Update(float deltaTime)
        {
            impl?.Invoke();
            Complete = true;
        }
    }

    public class ActionTests
    {
        [Test]
        public void Builder_Defaults_AreSet()
        {
            var action = new GoapAction.Builder().Build();

            var ctx = new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot());

            Assert.AreEqual(string.Empty, action.Name);
            Assert.AreEqual(string.Empty, action.Description);

            Assert.IsTrue(action.PreConditions(ctx));

            Assert.AreEqual(1f, action.Time(ctx));

            Assert.DoesNotThrow(() => action.Effect(ctx));
            Assert.DoesNotThrow(() => action.Implementation(new object(), ctx));
        }

        [Test]
        public void Builder_AssignsProperties()
        {
            var didRun = false;
            
            var action = new GoapAction.Builder()
                .WithName("Test")
                .WithDescription("Desc")
                .WithPreCondition(ctx => ctx.state.GetBelieve(0) > 0f)
                .WithTime(_ => 2.5f)
                .WithEffect(ctx =>
                {
                    var next = ctx.state.Clone();
                    next.SetBelieve(0, 1f);
                    return next;
                })
                .WithImplementation((_, __) => new StrategyHandler(() => didRun = true))
                .Build();

            var ctx = new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot());

            Assert.AreEqual("Test", action.Name);
            Assert.AreEqual("Desc", action.Description);

            Assert.IsFalse(action.PreConditions(ctx));
            Assert.AreEqual(2.5f, action.Time(ctx));
            
            var result = action.Effect(ctx);
            Assert.AreEqual(1f, result.GetBelieve(0));

            Assert.IsTrue(action.PreConditions(new AgentContext<TestWorldSnapshot>(result, new TestWorldSnapshot())));

            var strategy = action.Implementation(new object(), ctx);
            strategy.Update(0f);
            Assert.IsTrue(didRun);
        }

        [Test]
        public void Effect_UsesMutateToReturnUpdatedState()
        {
            var action = new GoapAction.Builder()
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(0, 1f);
                    })
                )
                .Build();

            var ctx = new AgentContext<TestWorldSnapshot>(new AgentState(1), new TestWorldSnapshot());
            var before = ctx.state.GetBelieve(0);

            var result = action.Effect(ctx);

            Assert.AreEqual(0f, before);
            Assert.AreEqual(0f, ctx.state.GetBelieve(0), "Effect must not mutate ctx.state in-place.");
            Assert.AreEqual(1f, result.GetBelieve(0), "Effect should return updated state.");
        }
        
        [Test]
        public void Effect_MustNotMutate_Input_State()
        {
            var action = new GoapAction.Builder()
                .WithEffect(ctx =>
                    ctx.state.Clone().Mutate((ref AgentState s) =>
                    {
                        s.SetBelieve(0, 1f);
                    })
                )
                .Build();

            var originalState = new AgentState(1);
            originalState.SetBelieve(0, 0f);

            var ctx = new AgentContext<TestWorldSnapshot>(originalState, new TestWorldSnapshot());
            var result = action.Effect(ctx);

            Assert.AreEqual(0f, ctx.state.GetBelieve(0), "Effect must not mutate the input context state.");
            Assert.AreEqual(1f, result.GetBelieve(0), "Effect must return a modified copy of the state.");
            Assert.IsFalse(result.Equals(ctx.state), "Result state should not equal original state after mutation.");
        }
    }
}


