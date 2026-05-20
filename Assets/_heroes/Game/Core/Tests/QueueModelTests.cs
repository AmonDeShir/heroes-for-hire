using System.Collections.Generic;
using Heroes.Game.Core.Models;
using NUnit.Framework;

namespace Heroes.Tests.Core
{
    public class QueueModelTests
    {
        [Test]
        public void Enqueue_IgnoresUnavailableItems()
        {
            var queue = new QueueModel(new List<string> { "a" });

            queue.Enqueue("b", 2f);

            Assert.That(queue.Queue, Is.Empty);
        }

        [Test]
        public void Progress_CompletesActiveItemAndStoresCompletedId()
        {
            var queue = new QueueModel(new List<string> { "a" });
            string completed = null;
            queue.OnCompleted += value => completed = value;

            queue.Enqueue("a", 1f);
            queue.Progress(0.5f);
            queue.Progress(0.6f);

            Assert.AreEqual("a", completed);
            Assert.AreEqual(1, queue.GetCompletedCount("a"));
            Assert.That(queue.Completed, Does.Contain("a"));
            Assert.That(queue.active, Is.Null);
        }

        [Test]
        public void Progress_ReportsNormalizedProgressFromZeroToOne()
        {
            var queue = new QueueModel(new List<string> { "a" });
            var samples = new List<float>();
            queue.OnProgressChanged += value => samples.Add(value);

            queue.Enqueue("a", 2f);
            queue.Progress(1f);
            queue.Progress(1f);

            Assert.That(samples.Count, Is.GreaterThanOrEqualTo(2));
            Assert.AreEqual(0.5f, samples[0], 0.001f);
            Assert.AreEqual(1f, samples[^1], 0.001f);
        }

        [Test]
        public void Enqueue_DoesNotRepeatCompletedItem_WhenRepeatDisabled()
        {
            var queue = new QueueModel(new List<string> { "a" });

            queue.Enqueue("a", 1f);
            queue.Progress(1f);
            queue.Enqueue("a", 1f);

            Assert.That(queue.Queue, Is.Empty);
            Assert.AreEqual(1, queue.GetCompletedCount("a"));
        }

        [Test]
        public void MakeAvailable_PublishesFullAvailableList()
        {
            var queue = new QueueModel(new List<string> { "a" });
            IReadOnlyList<string> published = null;
            queue.OnAvailableChanged += value => published = value;

            queue.MakeAvailable(new List<string> { "b" });

            Assert.That(queue.Available, Does.Contain("a"));
            Assert.That(queue.Available, Does.Contain("b"));
            Assert.That(published, Does.Contain("a"));
            Assert.That(published, Does.Contain("b"));
        }
    }
}


