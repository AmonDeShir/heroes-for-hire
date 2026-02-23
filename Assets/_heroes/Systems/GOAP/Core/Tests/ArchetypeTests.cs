using System.Collections.Generic;
using NUnit.Framework;
using GoapAction = Heroes.GOAP.Core.Action<object, Heroes.GOAP.Core.Tests.TestWorldSnapshot>;

namespace Heroes.GOAP.Core.Tests
{
    public class ArchetypeTests
    {
        [Test]
        public void CreateState_ReturnsCloneOfBaseState()
        {
            var baseState = new AgentState(1);
            baseState.SetBelieve(0, 0.25f);

            var archetype = new Archetype<object, TestWorldSnapshot>(new List<GoapAction>(), new List<Goal<TestWorldSnapshot>>(), baseState);

            var created = archetype.CreateState();
            created.SetBelieve(0, 0.75f);

            Assert.AreEqual(0.25f, baseState.GetBelieve(0));
            Assert.AreEqual(0.75f, created.GetBelieve(0));
        }
    }
}
