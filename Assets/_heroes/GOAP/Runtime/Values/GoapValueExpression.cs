using System;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Planner;
using UnityEngine;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public abstract class GoapValueExpression
    {
        public abstract GoapValue Evaluate(GoapState state);
    }

    [Serializable]
    public class GoapValueConstant : GoapValueExpression
    {
        public GoapValue Value;

        public override GoapValue Evaluate(GoapState state)
        {
            return Value;
        }
    }

    [Serializable]
    public class GoapValueVariable : GoapValueExpression
    {
        public string VariableName;
        public GoapValueType Type;

        public override GoapValue Evaluate(GoapState state)
        {
            if (state != null && state.TryGet(VariableName, out var value) && value.Type == Type)
                return value;

            return Type switch
            {
                GoapValueType.Float => GoapValue.FromFloat(0f),
                GoapValueType.Bool => GoapValue.FromBool(false),
                GoapValueType.Location => GoapValue.FromLocation(null),
                _ => default
            };
        }
    }

    public enum GoapFloatOp
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public enum GoapBoolOp
    {
        And,
        Or
    }

    [Serializable]
    public class GoapFloatBinaryOp : GoapValueExpression
    {
        public GoapFloatOp Operator;
        [SerializeReference] public GoapValueExpression A;
        [SerializeReference] public GoapValueExpression B;

        public override GoapValue Evaluate(GoapState state)
        {
            var aValue = A != null ? A.Evaluate(state) : default;
            var bValue = B != null ? B.Evaluate(state) : default;
            if (aValue.Type != GoapValueType.Float || bValue.Type != GoapValueType.Float)
                return GoapValue.FromFloat(0f);

            float result = 0f;
            switch (Operator)
            {
                case GoapFloatOp.Add:
                    result = aValue.FloatValue + bValue.FloatValue;
                    break;
                case GoapFloatOp.Subtract:
                    result = aValue.FloatValue - bValue.FloatValue;
                    break;
                case GoapFloatOp.Multiply:
                    result = aValue.FloatValue * bValue.FloatValue;
                    break;
                case GoapFloatOp.Divide:
                    result = Math.Abs(bValue.FloatValue) <= 0.000001f ? 0f : aValue.FloatValue / bValue.FloatValue;
                    break;
            }

            return GoapValue.FromFloat(result);
        }
    }

    [Serializable]
    public class GoapBoolBinaryOp : GoapValueExpression
    {
        public GoapBoolOp Operator;
        [SerializeReference] public GoapValueExpression A;
        [SerializeReference] public GoapValueExpression B;

        public override GoapValue Evaluate(GoapState state)
        {
            var aValue = A != null ? A.Evaluate(state) : default;
            var bValue = B != null ? B.Evaluate(state) : default;
            if (aValue.Type != GoapValueType.Bool || bValue.Type != GoapValueType.Bool)
                return GoapValue.FromBool(false);

            bool result = false;
            switch (Operator)
            {
                case GoapBoolOp.And:
                    result = aValue.BoolValue && bValue.BoolValue;
                    break;
                case GoapBoolOp.Or:
                    result = aValue.BoolValue || bValue.BoolValue;
                    break;
            }

            return GoapValue.FromBool(result);
        }
    }

    [Serializable]
    public class GoapBoolNot : GoapValueExpression
    {
        [SerializeReference] public GoapValueExpression Value;

        public override GoapValue Evaluate(GoapState state)
        {
            var value = Value != null ? Value.Evaluate(state) : default;
            if (value.Type != GoapValueType.Bool)
                return GoapValue.FromBool(false);

            return GoapValue.FromBool(!value.BoolValue);
        }
    }
}
