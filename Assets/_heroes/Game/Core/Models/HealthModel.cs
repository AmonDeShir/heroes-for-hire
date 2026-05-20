using EventBus;
using Heroes.Game.Core.Events;

namespace Heroes.Game.Core.Health
{
    public sealed class HealthModel
    {
        public string Id;
        public float Current { get; private set; }
        public float Max { get; private set; }

        public HealthModel(string id, float max, float start)
        {
            Id = id;
            Max = max < 0f ? 0f : max;
            SetCurrent(start);
        }

        public void SetMax(float max)
        {
            Max = max < 0f ? 0f : max;
            SetCurrent(Current);
        }

        public void SetCurrent(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }

            if (value > Max)
            {
                value = Max;
            }
            
            Current = value;
        }
    }
}


