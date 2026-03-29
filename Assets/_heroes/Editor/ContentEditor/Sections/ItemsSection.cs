using Heroes.Content.Definitions.Items;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class ItemsSection : ContentEditorSection
    {
        public override string Title => "Items";
        public override System.Type AssetType => typeof(ItemDefinition);
        public override string AssetFolder => ContentEditorPaths.ItemsFolder;
    }
}
