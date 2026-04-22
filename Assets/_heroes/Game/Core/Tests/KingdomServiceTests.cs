using Heroes.Game.Buildings;
using Heroes.Game.Core;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class KingdomServiceTests
    {
        [Test]
        public void TrySpendGold_ConsumesGoldThroughService()
        {
            var kingdom = new KingdomModel(100);
            var service = new KingdomService(kingdom);

            var result = service.TrySpendGold(25);

            Assert.IsTrue(result);
            Assert.AreEqual(75, kingdom.Gold);
        }

        [Test]
        public void AddPeople_UpdatesPopulationThroughService()
        {
            var kingdom = new KingdomModel(100);
            var service = new KingdomService(kingdom);

            service.AddPeople(2);

            Assert.AreEqual(3, kingdom.Population);
        }
    }
}
