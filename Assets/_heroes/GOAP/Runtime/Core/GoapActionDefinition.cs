using System;
using System.Collections.Generic;
using UnityEngine;
using Heroes.Goap.Runtime.Strategies;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.Core
{
    [Serializable]
    public class GoapActionDefinition
    {
        public string Id;
        public string Name;
        public float BaseCost;
        [SerializeReference] public List<GoapConditionNode> Preconditions = new List<GoapConditionNode>();
        public List<GoapEffect> Effects = new List<GoapEffect>();
        public List<GoapParameter> Parameters = new List<GoapParameter>();
        public GoapStrategyGraphAsset Strategy;
    }
}
