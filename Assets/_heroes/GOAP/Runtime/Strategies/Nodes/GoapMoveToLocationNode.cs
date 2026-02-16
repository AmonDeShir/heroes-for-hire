using System.Collections;
using Heroes.Goap.Runtime.World;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapMoveToLocationNode : GoapStrategyNode
    {
        public LocationSO Target;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (context.LocationExecutor != null)
                yield return context.LocationExecutor.MoveTo(Target);

            context.NextPort = GoapStrategyPortNames.Out;
        }
    }
}
