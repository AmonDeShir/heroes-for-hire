using Heroes.Game.Core.Health;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class HealthLogicTests
    {
        [Test]
        public void DamageLogic_ReducesHealth()
        {
            var health = new HealthModel("0", 100f, 50f);
            var damage = new DamageLogic(health);

            damage.Apply(15f);

            Assert.AreEqual(35f, health.Current);
        }

        [Test]
        public void HealLogic_IncreasesHealth()
        {
            var health = new HealthModel("0", 100f, 20f);
            var heal = new HealLogic(health);

            heal.Apply(25f);

            Assert.AreEqual(45f, health.Current);
        }
    }
}


