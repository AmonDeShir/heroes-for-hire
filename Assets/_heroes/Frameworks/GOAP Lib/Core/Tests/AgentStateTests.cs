using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Heroes.GOAP.Core.Tests
{
    public class AgentStateTests
    {
        [Test]
        public void Constructor_InitializesBelievesToZero()
        {
            var state = new AgentState(2);

            Assert.AreEqual(0f, state.GetBelieve(0));
            Assert.AreEqual(0f, state.GetBelieve(1));
        }

        [Test]
        public void SetAndGetBelieve_Works()
        {
            var state = new AgentState(2);
            state.SetBelieve(1, 0.75f);

            Assert.AreEqual(0.75f, state.GetBelieve(1));
        }

        [Test]
        public void GetBelieve_InvalidIndex_LogsWarningAndReturnsZero()
        {
            var state = new AgentState(2);
            var regex = new Regex("GOAP ERROR: believe id is incorrect.*");

            LogAssert.Expect(LogType.Warning, regex);

            Assert.AreEqual(0f, state.GetBelieve(5));
        }

        [Test]
        public void Equals_ReturnsTrueForSameValues()
        {
            var a = new AgentState(2);
            var b = new AgentState(2);

            a.SetBelieve(0, 0.25f);
            a.SetBelieve(1, 1f);

            b.SetBelieve(0, 0.25f);
            b.SetBelieve(1, 1f);

            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            var original = new AgentState(1);
            original.SetBelieve(0, 0.25f);

            var copy = original.Clone();
            copy.SetBelieve(0, 0.75f);

            Assert.AreEqual(0.25f, original.GetBelieve(0));
            Assert.AreEqual(0.75f, copy.GetBelieve(0));
        }
    }
}
