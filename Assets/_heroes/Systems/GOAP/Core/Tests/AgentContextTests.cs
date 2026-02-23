using NUnit.Framework;

namespace Heroes.GOAP.Core.Tests
{
    public class AgentContextTests
    {
        [Test]
        public void Constructor_ClonesInputState()
        {
            var state = new AgentState(1);
            state.SetBelieve(0, 0.25f);

            var ctx = new AgentContext<TestWorldSnapshot>(state, new TestWorldSnapshot());
            state.SetBelieve(0, 0.75f);

            Assert.AreEqual(0.25f, ctx.state.GetBelieve(0));
        }

        [Test]
        public void CopyConstructor_ClonesContextState()
        {
            var state = new AgentState(1);
            state.SetBelieve(0, 0.25f);

            var ctx = new AgentContext<TestWorldSnapshot>(state, new TestWorldSnapshot());
            var copy = new AgentContext<TestWorldSnapshot>(ctx);

            ctx.state.SetBelieve(0, 0.75f);

            Assert.AreEqual(0.25f, copy.state.GetBelieve(0));
        }
    }
}
