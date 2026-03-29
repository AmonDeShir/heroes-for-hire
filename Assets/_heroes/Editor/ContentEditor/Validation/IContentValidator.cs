using System.Collections.Generic;
using Heroes.Content.Definitions.Common;

namespace Heroes.Editor.ContentEditor.Validation
{
    public interface IContentValidator
    {
        bool CanValidate(DefinitionBase asset);
        IEnumerable<ContentValidationResult> Validate(DefinitionBase asset);
    }
}
