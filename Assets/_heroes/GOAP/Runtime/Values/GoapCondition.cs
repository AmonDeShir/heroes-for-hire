using System;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapCondition
    {
        public string VariableName;
        public GoapConditionOp Operator;
        public GoapValue Value;
    }
}
