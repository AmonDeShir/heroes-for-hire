using System.Collections.Generic;
using Heroes.Goap.Runtime.Core;

namespace Heroes.Goap.Runtime.Planner
{
    public class GoapPlan
    {
        public readonly List<GoapActionDefinition> Actions = new List<GoapActionDefinition>();
        public float TotalCost;
    }
}
