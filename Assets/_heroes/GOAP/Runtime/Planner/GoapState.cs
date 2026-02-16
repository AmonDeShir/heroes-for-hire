using System;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.Planner
{
    public class GoapState : IEquatable<GoapState>
    {
        readonly Dictionary<string, GoapValue> m_Values;
        int m_Hash;

        public GoapState()
        {
            m_Values = new Dictionary<string, GoapValue>();
        }

        GoapState(Dictionary<string, GoapValue> values, int hash)
        {
            m_Values = values;
            m_Hash = hash;
        }

        public IReadOnlyDictionary<string, GoapValue> Values => m_Values;

        public GoapState Clone()
        {
            return new GoapState(new Dictionary<string, GoapValue>(m_Values), m_Hash);
        }

        public void Set(string variableName, GoapValue value)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                return;

            m_Values[variableName] = value;
            m_Hash = 0;
        }

        public bool TryGet(string variableName, out GoapValue value)
        {
            return m_Values.TryGetValue(variableName, out value);
        }

        public bool Equals(GoapState other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null || m_Values.Count != other.m_Values.Count)
                return false;

            foreach (var pair in m_Values)
            {
                if (!other.m_Values.TryGetValue(pair.Key, out var otherValue))
                    return false;

                if (!GoapValueComparer.AreEqual(pair.Value, otherValue))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GoapState);
        }

        public override int GetHashCode()
        {
            if (m_Hash != 0)
                return m_Hash;

            unchecked
            {
                int hash = 17;
                foreach (var pair in m_Values)
                {
                    hash = hash * 31 + pair.Key.GetHashCode();
                    hash = hash * 31 + GoapValueComparer.GetHashCode(pair.Value);
                }
                m_Hash = hash;
            }

            return m_Hash;
        }
    }
}
