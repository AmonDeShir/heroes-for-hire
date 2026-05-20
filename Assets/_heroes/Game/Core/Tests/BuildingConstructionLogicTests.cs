using System.Collections.Generic;
using Heroes.Game.Buildings;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class BuildingConstructionLogicTests
    {
        [Test]
        public void Tick_IncreasesHpByRate()
        {
            var model = new BuildingModel("id", "def", new List<string>(), 100f, 0f);
            var logic = new BuildingConstructionLogic(model, 10f);

            logic.Tick(2f);

            Assert.AreEqual(20f, model.Health.Current);
            Assert.AreEqual(2, model.ConstructionStage);
        }
    }
}


