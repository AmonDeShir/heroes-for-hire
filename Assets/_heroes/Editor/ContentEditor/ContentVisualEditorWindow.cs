using System;
using System.Collections.Generic;
using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Definitions.Common;
using Heroes.Tools.BuildingCreator;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.Editor.ContentEditor
{
    public class ContentVisualEditorWindow : EditorWindow
    {
        private DefinitionBase asset;
        private BuildingCreatorPreview preview;
        private int iconSize = 512;

        private VisualElement root;

        [MenuItem("Tools/Heroes/Content Visual Editor")]
        public static void ShowWindow()
        {
            var window = CreateWindow<ContentVisualEditorWindow>("Content Visual Editor");
            window.minSize = new Vector2(780, 520);
        }

        public static void ShowWindow(DefinitionBase target)
        {
            var window = CreateWindow<ContentVisualEditorWindow>("Content Visual Editor");
            window.minSize = new Vector2(780, 520);
            window.SetAsset(target);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            BuildUI();
            SyncSelection();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            SyncSelection();
        }

        private void SyncSelection()
        {
            var selected = Selection.activeObject as DefinitionBase;
            if (selected != null)
            {
                SetAsset(selected);
            }
        }

        private void BuildUI()
        {
            rootVisualElement.Clear();
            root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            rootVisualElement.Add(root);

            var assetField = new ObjectField("Asset")
            {
                objectType = typeof(DefinitionBase),
                value = asset
            };
            assetField.RegisterValueChangedCallback(evt => SetAsset(evt.newValue as DefinitionBase));
            root.Add(assetField);

            var previewField = new ObjectField("Preview Host")
            {
                objectType = typeof(BuildingCreatorPreview),
                value = preview
            };
            previewField.RegisterValueChangedCallback(evt => preview = evt.newValue as BuildingCreatorPreview);
            root.Add(previewField);

            var sizeField = new IntegerField("Icon Size") { value = iconSize };
            sizeField.RegisterValueChangedCallback(evt => iconSize = evt.newValue);
            root.Add(sizeField);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            root.Add(row);

            row.Add(MakeButton("Open Preview Scene", () =>
            {
                BuildingPreviewSceneUtility.OpenOrCreateScene();
                preview = BuildingPreviewSceneUtility.GetOrCreatePreview(true);
            }));

            row.Add(MakeButton("Align Camera To Scene View", () =>
            {
                EnsurePreview();
                BuildingPreviewSceneUtility.AlignPreviewCameraToSceneView(preview);
            }));

            row.Add(MakeButton("Use Selected GameObject", UseSelectedObject));
            row.Add(MakeButton("Clear Preview", ClearPreview));
            row.Add(MakeButton("Capture Icon", CaptureIcon));

            BuildBuildingControls();
        }

        private void BuildBuildingControls()
        {
            var building = asset as BuildingDefinition;
            if (building == null)
            {
                return;
            }

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginTop = 6;
            root.Add(row);

            row.Add(MakeButton("Load Prefab Into Preview", () => LoadFromPrefab(building)));
            row.Add(MakeButton("Save Prefab From Preview", () => SavePrefab(building)));
            row.Add(MakeButton("Open Main Prefab", () => OpenMainPrefab(building)));
            row.Add(MakeButton("Capture Icon From Preview", () => CaptureBuildingIcon(building)));
            row.Add(MakeButton("Test Build", TestConstruction));
            row.Add(MakeButton("Test Destroy", TestDestruction));

        }

        private void SetAsset(DefinitionBase target)
        {
            asset = target;
            BuildUI();
        }

        private void UseSelectedObject()
        {
            EnsurePreview();
            if (preview == null)
            {
                EditorUtility.DisplayDialog("Missing Preview", "Open the preview scene first.", "OK");
                return;
            }

            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Missing Selection", "Select a GameObject in the scene.", "OK");
                return;
            }

            BuildingPreviewSceneUtility.ClearPreview(preview);
            var instance = Instantiate(selected, preview.PreviewRoot);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }

        private void ClearPreview()
        {
            EnsurePreview();
            BuildingPreviewSceneUtility.ClearPreview(preview);
        }

        private void CaptureIcon()
        {
            EnsurePreview();
            if (asset == null || preview == null)
            {
                return;
            }

            ContentEditorUtility.CaptureIconFromPreview(asset, preview, iconSize, ContentEditorPaths.IconsFolder, "Icons");
        }

        private void LoadFromPrefab(BuildingDefinition building)
        {
            EnsurePreview();
            if (preview == null)
            {
                return;
            }

            BuildingPreviewSceneUtility.SetPreviewFromPrefab(preview, building.Prefab);
        }

        private void SavePrefab(BuildingDefinition building)
        {
            EnsurePreview();
            if (preview == null)
            {
                return;
            }

            var prefab = BuildingPreviewSceneUtility.SavePrefabFromPreview(building, preview);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Missing Preview Object", "Add a preview object first.", "OK");
            }
        }

        private void OpenMainPrefab(BuildingDefinition building)
        {
            if (building == null || building.Prefab == null)
            {
                EditorUtility.DisplayDialog("Missing Prefab", "Assign a main prefab first.", "OK");
                return;
            }

            AssetDatabase.OpenAsset(building.Prefab);
        }

        private void CaptureBuildingIcon(BuildingDefinition building)
        {
            EnsurePreview();
            if (preview == null)
            {
                return;
            }

            ContentEditorUtility.CaptureBuildingIcon(building, preview, iconSize);
        }

        private void TestConstruction()
        {
            EnsurePreview();
            preview?.PlayConstructionSequence(1.2f);
        }

        private void TestDestruction()
        {
            EnsurePreview();
            preview?.PlayDestructionSequence(1.2f);
        }

        private void EnsurePreview()
        {
            if (preview == null)
            {
                preview = BuildingPreviewSceneUtility.GetOrCreatePreview(true);
            }
        }


        private static Button MakeButton(string text, Action action)
        {
            var button = new Button(action) { text = text };
            button.style.marginRight = 6;
            button.style.marginBottom = 6;
            return button;
        }
    }
}
