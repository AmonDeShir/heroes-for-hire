using System.Collections;
using UnityEngine;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapWaitNode : GoapStrategyNode
    {
        public float Duration;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (Duration > 0f)
                yield return new WaitForSeconds(Duration);

            context.NextPort = GoapStrategyPortNames.Out;
        }
    }
}
