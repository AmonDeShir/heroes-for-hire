using Heroes.Content.Definitions.Entities;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class EntitiesSection : ContentEditorSection
    {
        public override string Title => "Units";
        public override System.Type AssetType => typeof(EntityDefinition);
        public override string AssetFolder => ContentEditorPaths.EntitiesFolder;
    }
}
