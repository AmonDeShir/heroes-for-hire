using NUnit.Framework;

namespace Heroes.GOAP.Core.Tests
{
    public class AgentStateExtensionsTests
    {
        [Test]
        public void Mutate_UpdatesCopyAndReturnsNewState()
        {
            var original = new AgentState(1);

            var result = original.Clone()
                .Mutate((ref AgentState s) => s.SetBelieve(0, 1f));

            Assert.AreEqual(0f, original.GetBelieve(0));
            Assert.AreEqual(1f, result.GetBelieve(0));
        }

        [Test]
        public void Mutate_AllowsChainedCalls()
        {
            var original = new AgentState(2);

            var result = original.Clone()
                .Mutate((ref AgentState s) => s.SetBelieve(0, 0.25f))
                .Mutate((ref AgentState s) => s.SetBelieve(1, 0.75f));

            Assert.AreEqual(0f, original.GetBelieve(0));
            Assert.AreEqual(0f, original.GetBelieve(1));
            Assert.AreEqual(0.25f, result.GetBelieve(0), 1e-6f);
            Assert.AreEqual(0.75f, result.GetBelieve(1), 1e-6f);
        }

        [Test]
        public void Clamp_CapsValuesToRange()
        {
            var input = new AgentState(1);
            input.SetBelieve(0, 10f);

            var capped = input.Clone().Clamp(0, 5f);

            Assert.AreEqual(10f, input.GetBelieve(0), 1e-6f);
            Assert.AreEqual(5f, capped.GetBelieve(0), 1e-6f);

            var negative = input.Clone()
                .Mutate((ref AgentState s) => s.SetBelieve(0, -2f))
                .Clamp(0, 5f);

            Assert.AreEqual(0f, negative.GetBelieve(0), 1e-6f);
        }

        [Test]
        public void Bucket_RoundsToNearestStep_AndDoesNotMutateOriginal()
        {
            var original = new AgentState(1);
            original.SetBelieve(0, 2.1f);

            var roundedDown = original.Clone().Bucket(0, 0.5f);
            Assert.AreEqual(2.0f, roundedDown.GetBelieve(0), 1e-6f);

            var roundedUp = original.Clone()
                .Mutate((ref AgentState s) => s.SetBelieve(0, 2.3f))
                .Bucket(0, 0.5f);
            Assert.AreEqual(2.5f, roundedUp.GetBelieve(0), 1e-6f);

            Assert.AreEqual(2.1f, original.GetBelieve(0), 1e-6f);
        }
    }
}
