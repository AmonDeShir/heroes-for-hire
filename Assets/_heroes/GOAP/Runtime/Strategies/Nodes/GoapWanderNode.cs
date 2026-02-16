using System.Collections;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapWanderNode : GoapStrategyNode
    {
        public float Radius = 5f;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (context.WanderExecutor != null)
                yield return context.WanderExecutor.Wander(Radius);

            context.NextPort = GoapStrategyPortNames.Out;
        }
    }
}
