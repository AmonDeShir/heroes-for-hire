using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Entities;
using Heroes.Content.Definitions.Items;
using Heroes.Content.Definitions.Skills;
using UnityEngine.UIElements;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class BuildingSection : ContentEditorSection
    {
        public override string Title => "Buildings";
        public override System.Type AssetType => typeof(BuildingDefinition);
        public override string AssetFolder => ContentEditorPaths.BuildingsFolder;

        public override void BuildInspector(VisualElement root, DefinitionBase asset)
        {
            base.BuildInspector(root, asset);

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.flexWrap = Wrap.Wrap;
            header.style.marginBottom = 8;

            header.Add(MakeButton("New Skill", () => ContentEditorWindow.CreateWindowAndAsset(typeof(SkillDefinition), "New Skill")));
            header.Add(MakeButton("New Hero", () => ContentEditorWindow.CreateWindowAndAsset(typeof(HeroDefinition), "New Hero")));
            header.Add(MakeButton("New Upgrade", () => ContentEditorWindow.CreateWindowAndAsset(typeof(BuildingUpgradeDefinition), "New Upgrade")));
            header.Add(MakeButton("New Item", () => ContentEditorWindow.CreateWindowAndAsset(typeof(ItemDefinition), "New Item")));

            root.Insert(0, header);
        }

        private static Button MakeButton(string text, System.Action action)
        {
            var button = new Button(action) { text = text };
            button.style.marginRight = 6;
            button.style.marginBottom = 6;
            return button;
        }
    }
}
