using System.Collections;
using UnityEngine;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapRandomNode : GoapStrategyNode
    {
        [Range(0f, 1f)]
        public float ChanceA = 0.5f;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            context.NextPort = Random.value <= ChanceA ? GoapStrategyPortNames.A : GoapStrategyPortNames.B;
            yield break;
        }
    }
}
