using System;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;
using UnityEngine;

namespace Heroes.Goap.Runtime.Core
{
    [Serializable]
    public class GoapGoalDefinition
    {
        public string Id;
        public string Name;
        public float Priority;
        [SerializeReference] public List<GoapConditionNode> Desired = new List<GoapConditionNode>();
    }
}
