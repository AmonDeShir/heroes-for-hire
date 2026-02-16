using System;
using Heroes.Goap.Runtime.Strategies;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapStrategyVariableRef
    {
        public GoapStrategyGraphAsset Strategy;
        public string Name;
    }
}
