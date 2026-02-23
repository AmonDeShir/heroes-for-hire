using NUnit.Framework;

namespace Heroes.GOAP.Core.Tests
{
    public class WorldStateTests
    {
        [Test]
        public void CreateSnapshot_UsesCurrentVersionAndValidity()
        {
            var world = new TestWorldState();

            var snapshot = world.CreateSnapshot();
            Assert.AreEqual(0, snapshot.Version);
            Assert.IsTrue(snapshot.IsValid);

            world.SetValid(false);
            var updated = world.CreateSnapshot();

            Assert.AreEqual(1, updated.Version);
            Assert.IsFalse(updated.IsValid);
        }
    }
}
