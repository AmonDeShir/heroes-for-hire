using Heroes.Game.Core;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class KingdomModelTests
    {
        [Test]
        public void TrySpendGold_ConsumesWhenAffordable()
        {
            var kingdom = new KingdomModel(100);

            var result = kingdom.TrySpendGold(40);

            Assert.IsTrue(result);
            Assert.AreEqual(60, kingdom.Gold);
        }

        [Test]
        public void TrySpendGold_FailsWhenInsufficient()
        {
            var kingdom = new KingdomModel(10);

            var result = kingdom.TrySpendGold(20);

            Assert.IsFalse(result);
            Assert.AreEqual(10, kingdom.Gold);
        }
    }
}
