using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;

namespace Heroes.Goap.Runtime.World
{
    public class GoapMemoryState
    {
        readonly Dictionary<string, GoapValue> m_Values = new Dictionary<string, GoapValue>();

        public event System.Action<GoapMemoryChange> OnValueChanged;

        public IReadOnlyDictionary<string, GoapValue> Values => m_Values;

        public void Set(string variableName, GoapValue value)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                return;

            var hadOld = m_Values.TryGetValue(variableName, out var oldValue);
            m_Values[variableName] = value;

            OnValueChanged?.Invoke(new GoapMemoryChange(variableName, value, hadOld, oldValue));
        }

        public bool TryGet(string variableName, out GoapValue value)
        {
            return m_Values.TryGetValue(variableName, out value);
        }
    }

    public readonly struct GoapMemoryChange
    {
        public readonly string Name;
        public readonly GoapValue NewValue;
        public readonly bool HadOldValue;
        public readonly GoapValue OldValue;

        public GoapMemoryChange(string name, GoapValue newValue, bool hadOldValue, GoapValue oldValue)
        {
            Name = name;
            NewValue = newValue;
            HadOldValue = hadOldValue;
            OldValue = oldValue;
        }
    }
}
