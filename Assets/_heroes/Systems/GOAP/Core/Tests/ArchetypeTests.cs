using System.Collections.Generic;
using NUnit.Framework;
using GoapAction = Heroes.GOAP.Core.Action<object>;

namespace Heroes.GOAP.Core.Tests
{
    public class ArchetypeTests
    {
        [Test]
        public void CreateState_ReturnsCloneOfBaseState()
        {
            var baseState = new AgentState(1);
            baseState.SetBelieve(0, 0.25f);

            var archetype = new Archetype<object>(new List<GoapAction>(), new List<Goal>(), baseState);

            var created = archetype.CreateState();
            created.SetBelieve(0, 0.75f);

            Assert.AreEqual(0.25f, baseState.GetBelieve(0));
            Assert.AreEqual(0.75f, created.GetBelieve(0));
        }
    }
}
