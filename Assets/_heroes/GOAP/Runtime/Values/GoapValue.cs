using System;
using Heroes.Goap.Runtime.World;

namespace Heroes.Goap.Runtime.Values
{
    [Serializable]
    public struct GoapValue
    {
        public GoapValueType Type;
        public float FloatValue;
        public bool BoolValue;
        public LocationSO LocationValue;

        public static GoapValue FromFloat(float value)
        {
            return new GoapValue
            {
                Type = GoapValueType.Float,
                FloatValue = value
            };
        }

        public static GoapValue FromBool(bool value)
        {
            return new GoapValue
            {
                Type = GoapValueType.Bool,
                BoolValue = value
            };
        }

        public static GoapValue FromLocation(LocationSO value)
        {
            return new GoapValue
            {
                Type = GoapValueType.Location,
                LocationValue = value
            };
        }

        public bool TryGetFloat(out float value)
        {
            if (Type == GoapValueType.Float)
            {
                value = FloatValue;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetBool(out bool value)
        {
            if (Type == GoapValueType.Bool)
            {
                value = BoolValue;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetLocation(out LocationSO value)
        {
            if (Type == GoapValueType.Location)
            {
                value = LocationValue;
                return true;
            }

            value = default;
            return false;
        }

        public override string ToString()
        {
            return Type switch
            {
                GoapValueType.Float => FloatValue.ToString("0.###"),
                GoapValueType.Bool => BoolValue.ToString(),
                GoapValueType.Location => LocationValue ? LocationValue.name : "None",
                _ => string.Empty
            };
        }
    }
}
