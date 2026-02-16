using System;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapVariableRef
    {
        public string Name;

        public GoapVariableRef(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}
