using Heroes.Content.Definitions.Entities;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class HeroesSection : ContentEditorSection
    {
        public override string Title => "Heroes";
        public override System.Type AssetType => typeof(HeroDefinition);
        public override string AssetFolder => ContentEditorPaths.HeroesFolder;
    }
}
