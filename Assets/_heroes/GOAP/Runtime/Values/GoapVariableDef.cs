using System;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public class GoapVariableDef
    {
        public string Name;
        public GoapValueType Type;
        public GoapVariableScope Scope;
        public GoapValue DefaultValue;
    }
}
