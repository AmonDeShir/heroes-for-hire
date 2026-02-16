using System;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapParameter
    {
        public string VariableName;
        public GoapValue Value;
    }
}
