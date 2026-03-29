using Heroes.Content.Definitions.Effects;

namespace Heroes.Editor.ContentEditor.Sections
{
    public sealed class EffectsSection : ContentEditorSection
    {
        public override string Title => "Effects";
        public override System.Type AssetType => typeof(EffectDefinition);
        public override string AssetFolder => ContentEditorPaths.EffectsFolder;
    }
}
