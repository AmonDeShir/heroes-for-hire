using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;
using UnityEngine;

namespace Heroes.Goap.Runtime.Core
{
    [CreateAssetMenu(menuName = "Heroes/GOAP/Runtime Graph", fileName = "GoapGraph")]
    public class GoapGraphAsset : ScriptableObject
    {
        public List<GoapVariableDef> Variables = new List<GoapVariableDef>();
        public List<GoapActionDefinition> Actions = new List<GoapActionDefinition>();
        public List<GoapGoalDefinition> Goals = new List<GoapGoalDefinition>();
    }
}
