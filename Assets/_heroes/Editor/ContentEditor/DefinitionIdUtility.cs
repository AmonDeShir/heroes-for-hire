using System.Globalization;
using System.Text.RegularExpressions;

namespace Heroes.Editor.ContentEditor
{
    public static class DefinitionIdUtility
    {
        private static readonly Regex InvalidChars = new("[^a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string GenerateId(string displayName, string prefix)
        {
            var baseId = string.IsNullOrWhiteSpace(displayName)
                ? "new"
                : displayName.Trim().ToLower(CultureInfo.InvariantCulture);

            baseId = InvalidChars.Replace(baseId, "_");
            baseId = baseId.Trim('_');

            if (string.IsNullOrWhiteSpace(prefix))
            {
                return baseId;
            }

            return $"{prefix}_{baseId}";
        }
    }
}
