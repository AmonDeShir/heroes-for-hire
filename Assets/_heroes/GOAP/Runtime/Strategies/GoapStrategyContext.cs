using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;

namespace Heroes.Goap.Runtime.Strategies
{
    public interface IGoapAnimationExecutor
    {
        System.Collections.IEnumerator PlayAnimation(string animationName);
    }

    public interface IGoapLocationExecutor
    {
        System.Collections.IEnumerator MoveTo(LocationSO location);
    }

    public interface IGoapWanderExecutor
    {
        System.Collections.IEnumerator Wander(float radius);
    }

    public class GoapStrategyContext
    {
        public GoapWorldState World;
        public GoapMemoryState Memory;
        public IGoapAnimationExecutor Animation;
        public IGoapLocationExecutor LocationExecutor;
        public IGoapWanderExecutor WanderExecutor;
        public string NextPort = GoapStrategyPortNames.Out;

        public void SetValue(string variableName, GoapValue value)
        {
            Memory?.Set(variableName, value);
        }
    }

    public static class GoapStrategyPortNames
    {
        public const string In = "In";
        public const string Out = "Out";
        public const string True = "True";
        public const string False = "False";
        public const string A = "A";
        public const string B = "B";
    }
}
