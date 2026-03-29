using System.Collections.Generic;
using Heroes.Content.Definitions.Common;
using Heroes.Editor.ContentEditor.Validation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroes.Editor.ContentEditor
{
    public abstract class ContentEditorSection
    {
        public abstract string Title { get; }
        public abstract System.Type AssetType { get; }
        public abstract string AssetFolder { get; }

        public virtual void OnSelectionChanged(DefinitionBase asset)
        {
        }

        public virtual IEnumerable<DefinitionBase> LoadAssets()
        {
            return ContentEditorUtility.LoadAssets<DefinitionBase>(AssetFolder, AssetType);
        }

        public virtual void BuildInspector(VisualElement root, DefinitionBase asset)
        {
            root.Clear();
            if (asset == null)
            {
                root.Add(new Label("Select an asset to edit."));
                return;
            }

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.flexWrap = Wrap.Wrap;
            header.style.marginBottom = 8;

            var visualButton = new Button(() => ContentVisualEditorWindow.ShowWindow(asset))
            {
                text = "Open Visual Editor"
            };
            visualButton.style.marginRight = 6;
            header.Add(visualButton);

            root.Add(header);


            var so = new SerializedObject(asset);
            var iterator = so.GetIterator();
            var expanded = true;
            while (iterator.NextVisible(expanded))
            {
                if (iterator.propertyPath == "m_Script")
                {
                    expanded = false;
                    continue;
                }

                var field = new PropertyField(iterator);
                field.Bind(so);
                root.Add(field);
                expanded = false;
            }

            var footer = new VisualElement();
            footer.style.flexDirection = FlexDirection.Row;
            footer.style.marginTop = 8;

            var saveButton = new Button(() => AssetDatabase.SaveAssets()) { text = "Save" };
            var pingButton = new Button(() => EditorGUIUtility.PingObject(asset)) { text = "Ping" };
            footer.Add(saveButton);
            footer.Add(pingButton);
            root.Add(footer);

            BuildValidationSection(root, asset);
            BuildDependencySection(root, asset);
        }

        private static void BuildValidationSection(VisualElement root, DefinitionBase asset)
        {
            var results = ContentValidationRegistry.Validate(asset);
            var foldout = new Foldout { text = "Validation", value = false };

            if (results.Count == 0)
            {
                foldout.Add(new Label("No issues."));
                root.Add(foldout);
                return;
            }

            foreach (var result in results)
            {
                var label = new Label($"[{result.Severity}] {result.Message}");
                foldout.Add(label);
            }

            root.Add(foldout);
        }

        private static void BuildDependencySection(VisualElement root, DefinitionBase asset)
        {
            var foldout = new Foldout { text = "Dependencies", value = false };
            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrWhiteSpace(path))
            {
                foldout.Add(new Label("No asset path."));
                root.Add(foldout);
                return;
            }

            var dependencies = AssetDatabase.GetDependencies(path, false);
            foreach (var dependency in dependencies)
            {
                if (dependency == path)
                {
                    continue;
                }

                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dependency);
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 4;

                var label = new Label(dependency);
                label.style.flexGrow = 1f;
                var ping = new Button(() => EditorGUIUtility.PingObject(obj)) { text = "Ping" };

                row.Add(label);
                row.Add(ping);
                foldout.Add(row);
            }

            root.Add(foldout);
        }

    }
}
