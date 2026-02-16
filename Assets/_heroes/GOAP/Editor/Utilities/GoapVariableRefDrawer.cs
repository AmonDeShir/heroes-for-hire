using System.Collections.Generic;
using Heroes.Goap.Runtime.Values;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroes.Goap.Editor.Utilities
{
    [CustomPropertyDrawer(typeof(GoapVariableRef))]
    internal class GoapVariableRefDrawer : PropertyDrawer
    {
        const string NoneLabel = "(None)";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var nameProperty = property.FindPropertyRelative(nameof(GoapVariableRef.Name));
            var choices = BuildChoices();

            var dropdown = new DropdownField(property.displayName, choices, 0)
            {
                tooltip = property.tooltip
            };

            dropdown.value = GetDisplayName(nameProperty.stringValue, choices);

            dropdown.RegisterValueChangedCallback(evt =>
            {
                var value = evt.newValue == NoneLabel ? string.Empty : evt.newValue;
                nameProperty.stringValue = value;
                property.serializedObject.ApplyModifiedProperties();
            });

            return dropdown;
        }

        static List<string> BuildChoices()
        {
            var choices = GoapGraphEditorContext.GetVariableNamesFromFocusedGraph();
            choices.Insert(0, NoneLabel);
            return choices;
        }

        static string GetDisplayName(string value, List<string> choices)
        {
            if (string.IsNullOrWhiteSpace(value))
                return NoneLabel;

            if (choices.Contains(value))
                return value;

            choices.Add(value);
            return value;
        }
    }
}
