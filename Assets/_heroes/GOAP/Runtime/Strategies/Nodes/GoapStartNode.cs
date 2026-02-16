using System.Collections;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapStartNode : GoapStrategyNode
    {
        public override IEnumerator Execute(GoapStrategyContext context)
        {
            context.NextPort = GoapStrategyPortNames.Out;
            yield break;
        }
    }
}
