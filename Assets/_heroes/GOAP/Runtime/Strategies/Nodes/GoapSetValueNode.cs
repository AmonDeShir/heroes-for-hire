using System.Collections;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapSetValueNode : GoapStrategyNode
    {
        public string VariableName;
        public GoapValue Value;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            context.SetValue(VariableName, Value);
            context.NextPort = GoapStrategyPortNames.Out;
            yield break;
        }
    }
}
