using Heroes.Content.Definitions.Buildings;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class BuildingUpgradesSection : ContentEditorSection
    {
        public override string Title => "Upgrades";
        public override System.Type AssetType => typeof(BuildingUpgradeDefinition);
        public override string AssetFolder => ContentEditorPaths.UpgradesFolder;
    }
}
