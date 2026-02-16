using System.Collections;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapAddValueNode : GoapStrategyNode
    {
        public string VariableName;
        public float Delta;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (context.Memory != null && context.Memory.TryGet(VariableName, out var current) && current.Type == GoapValueType.Float)
            {
                context.SetValue(VariableName, GoapValue.FromFloat(current.FloatValue + Delta));
            }

            context.NextPort = GoapStrategyPortNames.Out;
            yield break;
        }
    }
}
