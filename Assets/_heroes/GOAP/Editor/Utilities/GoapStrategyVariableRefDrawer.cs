using System.Collections.Generic;
using Heroes.Goap.Editor.Graphs;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroes.Goap.Editor.Utilities
{
    [CustomPropertyDrawer(typeof(GoapStrategyVariableRef))]
    internal class GoapStrategyVariableRefDrawer : PropertyDrawer
    {
        const string NoneLabel = "(None)";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var strategyProperty = property.FindPropertyRelative(nameof(GoapStrategyVariableRef.Strategy));
            var nameProperty = property.FindPropertyRelative(nameof(GoapStrategyVariableRef.Name));

            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var strategyField = new ObjectField("Strategy")
            {
                objectType = typeof(Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset),
                allowSceneObjects = false,
                value = strategyProperty.objectReferenceValue
            };

            strategyField.RegisterValueChangedCallback(evt =>
            {
                strategyProperty.objectReferenceValue = evt.newValue as Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset;
                property.serializedObject.ApplyModifiedProperties();
                RefreshDropdown(root, nameProperty, strategyProperty.objectReferenceValue as Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset);
            });

            root.Add(strategyField);
            RefreshDropdown(root, nameProperty, strategyProperty.objectReferenceValue as Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset);

            return root;
        }

        static void RefreshDropdown(VisualElement root, SerializedProperty nameProperty, Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset strategy)
        {
            var existing = root.Q<DropdownField>("GoapStrategyVariableDropdown");
            if (existing != null)
                root.Remove(existing);

            var choices = BuildChoices(strategy);
            var dropdown = new DropdownField("Variable", choices, 0)
            {
                name = "GoapStrategyVariableDropdown"
            };

            dropdown.value = GetDisplayName(nameProperty.stringValue, choices);
            dropdown.RegisterValueChangedCallback(evt =>
            {
                nameProperty.stringValue = evt.newValue == NoneLabel ? string.Empty : evt.newValue;
                nameProperty.serializedObject.ApplyModifiedProperties();
            });

            root.Add(dropdown);
        }

        static List<string> BuildChoices(Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset strategy)
        {
            var choices = new List<string> { NoneLabel };
            if (strategy == null)
                return choices;

            var path = AssetDatabase.GetAssetPath(strategy);
            if (string.IsNullOrWhiteSpace(path))
                return choices;

            var graph = GraphDatabase.LoadGraph<GoapStrategyGraph>(path);
            if (graph == null)
                return choices;

            foreach (var variable in graph.GetVariables())
                choices.Add(variable.name);

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
