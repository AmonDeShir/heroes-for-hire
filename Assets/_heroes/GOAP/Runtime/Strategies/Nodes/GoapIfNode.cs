using System.Collections;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapIfNode : GoapStrategyNode
    {
        public GoapConditionOp Operator;
        public GoapValue Left;
        public GoapValue Right;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (GoapValueComparer.Evaluate(new GoapCondition { Operator = Operator, Value = Right }, Left))
            {
                context.NextPort = GoapStrategyPortNames.True;
            }
            else
            {
                context.NextPort = GoapStrategyPortNames.False;
            }

            yield break;
        }
    }
}
