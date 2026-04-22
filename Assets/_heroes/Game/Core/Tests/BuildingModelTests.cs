using System.Collections.Generic;
using Heroes.Game.Buildings;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class BuildingModelTests
    {
        [Test]
        public void SyncFromHealth_SetsStageBasedOnHp()
        {
            var model = new BuildingModel("id", "def", new List<string>(), 100f, 50f);
            model.SyncFromHealth();

            Assert.AreEqual(5, model.ConstructionStage);
            Assert.AreEqual(BuildingState.UnderConstruction, model.State);
        }

        [Test]
        public void SyncFromHealth_SetsCompletedWhenFullHp()
        {
            var model = new BuildingModel("id", "def", new List<string>(),100f, 100f);
            model.SyncFromHealth();

            Assert.AreEqual(10, model.ConstructionStage);
            Assert.AreEqual(BuildingState.Completed, model.State);
        }

        [Test]
        public void SyncFromHealth_SetsDamagedAfterCompletion()
        {
            var model = new BuildingModel("id", "def", new List<string>(), 100f, 100f);
            model.SyncFromHealth();

            model.Health.SetCurrent(60f);
            model.SyncFromHealth();

            Assert.AreEqual(BuildingState.Damaged, model.State);
        }

        [Test]
        public void SyncFromHealth_SetsDestroyedWhenZeroHp()
        {
            var model = new BuildingModel("id", "def", new List<string>(), 100f, 0f);
            model.SyncFromHealth();

            Assert.AreEqual(BuildingState.Destroyed, model.State);
        }
    }
}
