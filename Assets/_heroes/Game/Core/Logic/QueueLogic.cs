using System.Collections.Generic;
using Heroes.Game.Core.Models;
using EventBus;

namespace Heroes.Game.Core.Logic
{
    public class QueueLogic<TProgressEvent, TQueueEvent, TActiveEvent, TAvailableEvent, TCompletedEvent> 
        where TProgressEvent : IValueChangedWithIdEvent<float>, new() 
        where TQueueEvent : IValueChangedWithIdEvent<IReadOnlyList<string>>, new()
        where TActiveEvent : IValueChangedWithIdEvent<string>, new()
        where TAvailableEvent : IValueChangedWithIdEvent<IReadOnlyList<string>>, new()
        where TCompletedEvent : IValueChangedWithIdEvent<string>, new()
    {
        private QueueModel queue;
        private string buildingId;
        
        public QueueLogic(QueueModel queue, string buildingId)
        {
            this.queue = queue;
            this.buildingId = buildingId;
            
            queue.OnProgressChanged += QueueOnOnProgressChanged;
            queue.OnQueueChanged += QueueOnOnQueueChanged;
            queue.OnActiveChanged += QueueOnOnActiveChanged;
            queue.OnAvailableChanged += QueueOnOnAvailableChanged;
            queue.OnCompleted += QueueOnOnCompleted;
        }

        private void Destroy()
        {
            queue.OnProgressChanged -= QueueOnOnProgressChanged;
            queue.OnQueueChanged -= QueueOnOnQueueChanged;
            queue.OnActiveChanged -= QueueOnOnActiveChanged;
            queue.OnAvailableChanged -= QueueOnOnAvailableChanged;
            queue.OnCompleted -= QueueOnOnCompleted;
        }

        private void QueueOnOnCompleted(string value)
        {
            EventBus<TCompletedEvent>.Invoke(new TCompletedEvent { Value = value, Id = buildingId });
        }

        private void QueueOnOnAvailableChanged(IReadOnlyList<string> value)
        {
            EventBus<TAvailableEvent>.Invoke(new TAvailableEvent { Value = value, Id = buildingId  });
        }

        private void QueueOnOnActiveChanged(string value)
        {
            EventBus<TActiveEvent>.Invoke(new TActiveEvent { Value = value, Id = buildingId  });
        }

        private void QueueOnOnQueueChanged(IReadOnlyList<string> value)
        {
            EventBus<TQueueEvent>.Invoke(new TQueueEvent { Value = value, Id = buildingId  });
        }

        private void QueueOnOnProgressChanged(float value)
        {
            EventBus<TProgressEvent>.Invoke(new TProgressEvent { Value = value, Id = buildingId });
        }

        public void Tick(float dt)
        {
            queue.Progress(dt);
        }
    }
}


