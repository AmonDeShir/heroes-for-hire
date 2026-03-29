using System;
using System.Collections.Generic;
using System.Linq;
using Heroes.Content.Definitions.Common;
using Heroes.Editor.ContentEditor.Sections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.Editor.ContentEditor
{
    public class ContentEditorWindow : EditorWindow
    {
        private readonly List<ContentEditorSection> sections = new();
        private ContentEditorSection activeSection;
        private DefinitionBase selectedAsset;
        private DefinitionBase pendingSelection;

        private ListView listView;
        private VisualElement inspectorRoot;
        private TextField searchField;

        [MenuItem("Tools/Heroes/Content Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ContentEditorWindow>("Content Editor");
            window.minSize = new Vector2(900, 600);
        }

        public static ContentEditorWindow CreateWindowForType(Type assetType)
        {
            var window = CreateWindow<ContentEditorWindow>("Content Editor");
            window.minSize = new Vector2(900, 600);
            window.SetActiveSectionByType(assetType);
            return window;
        }

        public static ContentEditorWindow CreateWindowAndAsset(Type assetType, string displayName)
        {
            var window = CreateWindowForType(assetType);
            var asset = ContentEditorUtility.CreateAssetForType(assetType, displayName);
            if (asset != null)
            {
                window.SelectAsset(asset);
            }

            return window;
        }

        private void OnEnable()
        {
            if (sections.Count == 0)
            {
                sections.Add(new BuildingSection());
                sections.Add(new BuildingUpgradesSection());
                sections.Add(new EntitiesSection());
                sections.Add(new HeroesSection());
                sections.Add(new ItemsSection());
                sections.Add(new SkillsSection());
                sections.Add(new EffectsSection());
            }

            activeSection ??= sections[0];
            BuildUI();
            RefreshList();

            if (pendingSelection != null)
            {
                SelectAsset(pendingSelection);
                pendingSelection = null;
            }
        }

        private void BuildUI()
        {
            rootVisualElement.Clear();

            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            foreach (var section in sections)
            {
                var toggle = new ToolbarToggle
                {
                    text = section.Title,
                    value = section == activeSection
                };

                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (!evt.newValue)
                    {
                        return;
                    }

                    foreach (var other in toolbar.Children())
                    {
                        if (other is ToolbarToggle otherToggle && otherToggle != toggle)
                        {
                            otherToggle.SetValueWithoutNotify(false);
                        }
                    }

                    activeSection = section;
                    RefreshList();
                    UpdateInspector();
                });

                toolbar.Add(toggle);
            }

            toolbar.Add(new ToolbarSpacer());

            var duplicateButton = new ToolbarButton(() => DuplicateAsset()) { text = "Duplicate" };
            var deleteButton = new ToolbarButton(() => DeleteAsset()) { text = "Delete" };
            var regenerateButton = new ToolbarButton(() => RegenerateId()) { text = "Regenerate Id" };
            toolbar.Add(duplicateButton);
            toolbar.Add(deleteButton);
            toolbar.Add(regenerateButton);

            var rootSplit = new TwoPaneSplitView(0, 360, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(rootSplit);

            var listPane = new VisualElement();
            listPane.style.flexDirection = FlexDirection.Column;
            rootSplit.Add(listPane);

            searchField = new TextField { name = "content-search", label = "Search" };
            searchField.RegisterValueChangedCallback(_ => RefreshList());
            listPane.Add(searchField);

            var createRow = new VisualElement();
            createRow.style.flexDirection = FlexDirection.Row;
            createRow.style.marginBottom = 6;

            var createNameField = new TextField { value = "New" };
            createNameField.style.flexGrow = 1f;
            var createButton = new Button(() => CreateAsset(createNameField.value)) { text = "Create" };
            createRow.Add(createNameField);
            createRow.Add(createButton);
            listPane.Add(createRow);

            listView = new ListView
            {
                selectionType = SelectionType.Single,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                style = { flexGrow = 1f }
            };

            listView.makeItem = () => new Label();
            listView.bindItem = (element, i) =>
            {
                var data = (DefinitionBase)listView.itemsSource[i];
                (element as Label).text = data != null ? data.DisplayName : "(missing)";
            };
            listView.onSelectionChange += OnSelectionChanged;
            listPane.Add(listView);

            inspectorRoot = new ScrollView
            {
                style = { flexGrow = 1f }
            };
            rootSplit.Add(inspectorRoot);
        }

        public void SetActiveSectionByType(Type assetType)
        {
            if (assetType == null)
            {
                return;
            }

            var section = sections.FirstOrDefault(x => x.AssetType == assetType);
            if (section == null)
            {
                return;
            }

            activeSection = section;
            BuildUI();
            RefreshList();
        }

        private void RefreshList()
        {
            if (activeSection == null || listView == null)
            {
                return;
            }

            var items = activeSection.LoadAssets().ToList();
            var query = searchField?.value;
            if (!string.IsNullOrWhiteSpace(query))
            {
                items = items
                    .Where(x => x.DisplayName != null && x.DisplayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            listView.itemsSource = items;
            listView.Rebuild();

            if (items.Count == 0)
            {
                selectedAsset = null;
            }
            else if (selectedAsset == null || !items.Contains(selectedAsset))
            {
                selectedAsset = items[0];
                listView.SetSelection(0);
            }

            UpdateInspector();
        }

        private void OnSelectionChanged(IEnumerable<object> selection)
        {
            foreach (var item in selection)
            {
                selectedAsset = item as DefinitionBase;
                activeSection?.OnSelectionChanged(selectedAsset);
                UpdateInspector();
                break;
            }
        }

        private void UpdateInspector()
        {
            if (activeSection == null || inspectorRoot == null)
            {
                return;
            }

            activeSection.BuildInspector(inspectorRoot, selectedAsset);
        }

        private void CreateAsset(string displayName)
        {
            if (activeSection == null)
            {
                return;
            }

            var name = string.IsNullOrWhiteSpace(displayName) ? "New" : displayName;
            var asset = ContentEditorUtility.CreateAsset(activeSection.AssetType, activeSection.AssetFolder, name, activeSection.Title.ToLowerInvariant());
            if (asset != null)
            {
                RefreshList();
                SelectAsset(asset);
            }
        }

        private void DuplicateAsset()
        {
            if (activeSection == null || selectedAsset == null)
            {
                return;
            }

            var copy = ContentEditorUtility.DuplicateAsset(selectedAsset, activeSection.AssetFolder);
            if (copy != null)
            {
                RefreshList();
                SelectAsset(copy);
            }
        }

        private void DeleteAsset()
        {
            if (selectedAsset == null)
            {
                return;
            }

            if (!EditorUtility.DisplayDialog("Delete Asset", $"Delete {selectedAsset.DisplayName}?", "Delete", "Cancel"))
            {
                return;
            }

            ContentEditorUtility.DeleteAsset(selectedAsset);
            selectedAsset = null;
            RefreshList();
        }

        private void RegenerateId()
        {
            if (selectedAsset == null || activeSection == null)
            {
                return;
            }

            ContentEditorUtility.RegenerateId(selectedAsset, activeSection.Title.ToLowerInvariant());
            RefreshList();
        }

        public void SelectAsset(DefinitionBase asset)
        {
            if (asset == null)
            {
                return;
            }

            if (listView?.itemsSource == null)
            {
                pendingSelection = asset;
                return;
            }

            var items = listView.itemsSource as IList<DefinitionBase>;
            if (items == null)
            {
                return;
            }

            var index = items.IndexOf(asset);
            if (index >= 0)
            {
                listView.SetSelection(index);
            }
        }
    }
}
