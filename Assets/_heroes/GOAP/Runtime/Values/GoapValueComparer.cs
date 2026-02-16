namespace Heroes.Goap.Runtime.Values
{
    public static class GoapValueComparer
    {
        const float k_FloatTolerance = 0.0001f;

        public static bool AreEqual(GoapValue a, GoapValue b)
        {
            if (a.Type != b.Type)
                return false;

            switch (a.Type)
            {
                case GoapValueType.Float:
                    return System.Math.Abs(a.FloatValue - b.FloatValue) <= k_FloatTolerance;
                case GoapValueType.Bool:
                    return a.BoolValue == b.BoolValue;
                case GoapValueType.Location:
                    return a.LocationValue == b.LocationValue;
                default:
                    return false;
            }
        }

        public static int GetHashCode(GoapValue value)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (int)value.Type;
                switch (value.Type)
                {
                    case GoapValueType.Float:
                        hash = hash * 31 + value.FloatValue.GetHashCode();
                        break;
                    case GoapValueType.Bool:
                        hash = hash * 31 + value.BoolValue.GetHashCode();
                        break;
                    case GoapValueType.Location:
                        hash = hash * 31 + (value.LocationValue ? value.LocationValue.GetHashCode() : 0);
                        break;
                }
                return hash;
            }
        }

        public static bool Evaluate(GoapCondition condition, GoapValue actual)
        {
            if (actual.Type != condition.Value.Type)
                return false;

            switch (condition.Value.Type)
            {
                case GoapValueType.Float:
                    return EvaluateFloat(condition.Operator, actual.FloatValue, condition.Value.FloatValue);
                case GoapValueType.Bool:
                    return EvaluateBool(condition.Operator, actual.BoolValue, condition.Value.BoolValue);
                case GoapValueType.Location:
                    return EvaluateLocation(condition.Operator, actual.LocationValue, condition.Value.LocationValue);
                default:
                    return false;
            }
        }

        static bool EvaluateFloat(GoapConditionOp op, float actual, float desired)
        {
            switch (op)
            {
                case GoapConditionOp.Equal:
                    return System.Math.Abs(actual - desired) <= k_FloatTolerance;
                case GoapConditionOp.NotEqual:
                    return System.Math.Abs(actual - desired) > k_FloatTolerance;
                case GoapConditionOp.Greater:
                    return actual > desired;
                case GoapConditionOp.GreaterOrEqual:
                    return actual >= desired;
                case GoapConditionOp.Less:
                    return actual < desired;
                case GoapConditionOp.LessOrEqual:
                    return actual <= desired;
                default:
                    return false;
            }
        }

        static bool EvaluateBool(GoapConditionOp op, bool actual, bool desired)
        {
            switch (op)
            {
                case GoapConditionOp.Equal:
                    return actual == desired;
                case GoapConditionOp.NotEqual:
                    return actual != desired;
                default:
                    return false;
            }
        }

        static bool EvaluateLocation(GoapConditionOp op, object actual, object desired)
        {
            switch (op)
            {
                case GoapConditionOp.Equal:
                    return actual == desired;
                case GoapConditionOp.NotEqual:
                    return actual != desired;
                default:
                    return false;
            }
        }
    }
}
