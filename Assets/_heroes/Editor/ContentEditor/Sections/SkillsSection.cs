using Heroes.Content.Definitions.Skills;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class SkillsSection : ContentEditorSection
    {
        public override string Title => "Skills";
        public override System.Type AssetType => typeof(SkillDefinition);
        public override string AssetFolder => ContentEditorPaths.SkillsFolder;
    }
}
