using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroes.Game.Core.Models
{
    public class QueueModel
    {
        private readonly HashSet<string> available;
        private readonly Dictionary<string, int> completedCounts;
        private readonly Queue<(string, float)> queue;

        private float processTime = 0f;
        private float startProcessTime = 0f;

        public string active { get; private set; }
        public float progress { get; private set; }
        public IReadOnlyList<string> Queue => queue.ToList().Select(x => x.Item1).ToList();
        public IReadOnlyList<string> Available => available.ToList();
        public IReadOnlyList<string> Completed => completedCounts.Keys.ToList();

        public QueueModel(List<string> available)
        {
            queue = new Queue<(string, float)>();
            completedCounts = new Dictionary<string, int>();
            this.available = new HashSet<string>(available ?? new List<string>());
        }
        
        public event Action<string> OnActiveChanged;
        public event Action<string> OnCompleted;
        public event Action<float> OnProgressChanged;
        public event Action<IReadOnlyList<string>> OnQueueChanged;
        public event Action<IReadOnlyList<string>> OnAvailableChanged;

        public void MakeAvailable(List<string> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                available.Add(item);
            }

            OnAvailableChanged?.Invoke(Available);
        }
        
        public void Enqueue(string id, float processTime, bool canRepeat = false)
        {
            if (!available.Contains(id))
            {
                Debug.LogError($"Item {id} is not available for this queue");
                return;
            }

            if (!canRepeat && WillRepeat(id, processTime))
            {
                return;
            }

            queue.Enqueue((id, processTime));
            OnQueueChanged?.Invoke(Queue);
        }

        public void Progress(float dt)
        {
            if (active == null && queue.TryDequeue(out var item))
            {
                var (nextId, nextTime) = item;
                
                active = nextId;
                processTime = Mathf.Max(nextTime, 0.0001f);
                startProcessTime = processTime;
                progress = 0f;
                
                OnActiveChanged?.Invoke(active);
                OnQueueChanged?.Invoke(Queue);
            }

            if (active == null)
            {
                return;
            }

            processTime -= dt;
            progress = 1f - (processTime / startProcessTime);

            if (progress < 0f)
            {
                progress = 0f;
            }
            else if (progress > 1f)
            {
                progress = 1f;
            }

            OnProgressChanged?.Invoke(progress);

            if (processTime <= 0f)
            {
                var completedId = active;
                active = null;
                progress = 1f;

                if (!string.IsNullOrWhiteSpace(completedId))
                {
                    completedCounts.TryGetValue(completedId, out var completedCount);
                    completedCounts[completedId] = completedCount + 1;
                    OnCompleted?.Invoke(completedId);
                }

                OnActiveChanged?.Invoke(null);
            }
        }

        public int GetCompletedCount(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return 0;
            }

            return completedCounts.TryGetValue(id, out var count) ? count : 0;
        }

        private bool WillRepeat(string id, float time)
        {
            return active == id || queue.Any(item => item.Item1 == id) || GetCompletedCount(id) > 0;
        }
    }
}


