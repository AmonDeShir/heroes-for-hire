using System;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Planner;
using UnityEngine;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public abstract class GoapConditionNode
    {
        public abstract bool Evaluate(GoapState state);
    }

    [Serializable]
    public class GoapConditionCompare : GoapConditionNode
    {
        public GoapConditionOp Operator;
        [SerializeReference] public GoapValueExpression Left;
        [SerializeReference] public GoapValueExpression Right;

        public override bool Evaluate(GoapState state)
        {
            var left = Left != null ? Left.Evaluate(state) : default;
            var right = Right != null ? Right.Evaluate(state) : default;
            return GoapValueComparer.Evaluate(new GoapCondition { Operator = Operator, Value = right }, left);
        }
    }

    [Serializable]
    public class GoapConditionAnd : GoapConditionNode
    {
        [SerializeReference] public List<GoapConditionNode> Conditions = new List<GoapConditionNode>();

        public override bool Evaluate(GoapState state)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i] == null || !Conditions[i].Evaluate(state))
                    return false;
            }

            return Conditions.Count > 0;
        }
    }

    [Serializable]
    public class GoapConditionOr : GoapConditionNode
    {
        [SerializeReference] public List<GoapConditionNode> Conditions = new List<GoapConditionNode>();

        public override bool Evaluate(GoapState state)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i] != null && Conditions[i].Evaluate(state))
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    public class GoapConditionNot : GoapConditionNode
    {
        [SerializeReference] public GoapConditionNode Condition;

        public override bool Evaluate(GoapState state)
        {
            return Condition != null && !Condition.Evaluate(state);
        }
    }
}
