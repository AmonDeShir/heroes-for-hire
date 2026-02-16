using System;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapEffect
    {
        public string VariableName;
        public GoapEffectOp Operator;
        public GoapValue Value;
    }
}
