using System;
using NUnit.Framework;

namespace Heroes.GOAP.Core.Tests
{
    public class TranspositionTableTests
    {
        [Test]
        public void Constructor_ThrowsWhenSizeIsNonPositive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TranspositionTable(0));
        }

        [Test]
        public void HasBetterOrEqual_ReturnsFalseWhenEmpty()
        {
            var table = new TranspositionTable(8);
            var state = new AgentState(1);

            Assert.IsFalse(table.HasBetterOrEqual(state, 1f));
        }

        [Test]
        public void AddOrImprove_StoresCostAndHasBetterOrEqualRespectsThreshold()
        {
            var table = new TranspositionTable(8);
            var state = new AgentState(1);
            state.SetBelieve(0, 0.5f);

            table.AddOrImprove(state, 3f);

            Assert.IsTrue(table.HasBetterOrEqual(state, 3f));
            Assert.IsTrue(table.HasBetterOrEqual(state, 4f));
            Assert.IsFalse(table.HasBetterOrEqual(state, 2f));
        }

        [Test]
        public void AddOrImprove_UpdatesBestCostForSameState()
        {
            var table = new TranspositionTable(8);
            var state = new AgentState(1);
            state.SetBelieve(0, 0.25f);

            table.AddOrImprove(state, 5f);
            table.AddOrImprove(state, 2f);

            Assert.IsTrue(table.HasBetterOrEqual(state, 2f));
            Assert.IsTrue(table.HasBetterOrEqual(state, 3f));
            Assert.IsFalse(table.HasBetterOrEqual(state, 1f));
        }

        [Test]
        public void AddOrImprove_ReplacesOnCollisionWhenNewCostIsLower()
        {
            var table = new TranspositionTable(1);
            var first = new AgentState(1);
            var second = new AgentState(1);

            first.SetBelieve(0, 0.25f);
            second.SetBelieve(0, 0.75f);

            table.AddOrImprove(first, 5f);
            table.AddOrImprove(second, 2f);

            Assert.IsFalse(table.HasBetterOrEqual(first, 5f));
            Assert.IsTrue(table.HasBetterOrEqual(second, 2f));
        }

        [Test]
        public void AddOrImprove_DoesNotReplaceOnCollisionWhenNewCostIsHigher()
        {
            var table = new TranspositionTable(1);
            var first = new AgentState(1);
            var second = new AgentState(1);

            first.SetBelieve(0, 0.25f);
            second.SetBelieve(0, 0.75f);

            table.AddOrImprove(first, 2f);
            table.AddOrImprove(second, 5f);

            Assert.IsTrue(table.HasBetterOrEqual(first, 2f));
            Assert.IsFalse(table.HasBetterOrEqual(second, 5f));
        }

        [Test]
        public void Clear_RemovesStoredEntries()
        {
            var table = new TranspositionTable(8);
            var state = new AgentState(1);
            state.SetBelieve(0, 0.5f);

            table.AddOrImprove(state, 1f);
            table.Clear();

            Assert.IsFalse(table.HasBetterOrEqual(state, 1f));
        }
    }
}
