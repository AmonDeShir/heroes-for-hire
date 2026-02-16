using System.IO;
using Heroes.Goap.Runtime.Strategies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.Goap.Editor.Utilities
{
    [CustomPropertyDrawer(typeof(GoapStrategyGraphAsset))]
    internal class GoapStrategyGraphAssetDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;

            var objectField = new ObjectField(property.displayName)
            {
                objectType = typeof(GoapStrategyGraphAsset),
                allowSceneObjects = false,
                value = property.objectReferenceValue
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                property.objectReferenceValue = evt.newValue as GoapStrategyGraphAsset;
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(objectField);

            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;

            var createButton = new Button(() => CreateStrategy(property, objectField))
            {
                text = "Create"
            };

            var openButton = new Button(() => OpenStrategy(property))
            {
                text = "Open"
            };

            var selectButton = new Button(() => SelectStrategy(property))
            {
                text = "Select"
            };

            buttonRow.Add(createButton);
            buttonRow.Add(openButton);
            buttonRow.Add(selectButton);

            container.Add(buttonRow);

            return container;
        }

        static void CreateStrategy(SerializedProperty property, ObjectField objectField)
        {
            var graphPath = GoapGraphEditorContext.GetFocusedGraphPath();
            var folder = string.IsNullOrWhiteSpace(graphPath) ? "Assets" : Path.GetDirectoryName(graphPath);
            var created = GoapGraphAssetCreator.CreateStrategyGraphAsset(folder, "Strategy");
            if (created == null)
                return;

            property.objectReferenceValue = created;
            property.serializedObject.ApplyModifiedProperties();
            objectField.SetValueWithoutNotify(created);
            EditorGUIUtility.PingObject(created);
        }

        static void OpenStrategy(SerializedProperty property)
        {
            var asset = property.objectReferenceValue as GoapStrategyGraphAsset;
            if (asset == null)
                return;

            var path = AssetDatabase.GetAssetPath(asset);
            if (!string.IsNullOrWhiteSpace(path))
                AssetDatabase.OpenAsset(asset);
        }

        static void SelectStrategy(SerializedProperty property)
        {
            var asset = property.objectReferenceValue as GoapStrategyGraphAsset;
            if (asset == null)
                return;

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }
}
