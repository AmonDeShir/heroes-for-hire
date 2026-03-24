using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor
{
    [CustomPropertyDrawer(typeof(GraphBeliefRef))]
    internal sealed class GraphBeliefRefDrawer : PropertyDrawer
    {
        private const string NoneLabel = "None";
        private static readonly List<string> NoneList = new List<string> { NoneLabel };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var nameProperty = property.FindPropertyRelative("name");
            var graph = TryGetGraph(property.serializedObject);
            var validNames = GoapBeliefNameRegistry.GetValidNames(graph);

            var choices = new List<string> { NoneLabel };
            if (validNames != null && validNames.Count > 0)
            {
                choices.AddRange(validNames);
            }

            var currentValue = nameProperty?.stringValue ?? string.Empty;
            var displayValue = IsValidSelection(validNames, currentValue) ? currentValue : NoneLabel;

            var field = new PopupField<string>(string.Empty, choices, displayValue);
            field.style.flexGrow = 1f;
            field.style.flexShrink = 1f;
            field.labelElement.style.display = DisplayStyle.None;
            field.RegisterValueChangedCallback(evt =>
            {
                if (nameProperty == null)
                {
                    return;
                }

                nameProperty.stringValue = string.Equals(evt.newValue, NoneLabel, StringComparison.Ordinal)
                    ? string.Empty
                    : evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            return field;
        }

        private static bool IsValidSelection(IReadOnlyList<string> validNames, string value)
        {
            if (validNames == null || validNames.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < validNames.Count; i++)
            {
                if (string.Equals(validNames[i], value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static Graph TryGetGraph(SerializedObject serializedObject)
        {
            var target = serializedObject?.targetObject;
            if (target == null)
            {
                return null;
            }

            var ownerProperty = target.GetType().GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var owner = ownerProperty?.GetValue(target);
            if (owner == null)
            {
                return null;
            }

            var graphModelProperty = owner.GetType().GetProperty("GraphModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var graphModel = graphModelProperty?.GetValue(owner);
            if (graphModel == null)
            {
                return null;
            }

            var graphProperty = graphModel.GetType().GetProperty("Graph", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return graphProperty?.GetValue(graphModel) as Graph;
        }
    }
}
