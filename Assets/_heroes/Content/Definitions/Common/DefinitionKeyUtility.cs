using System.Globalization;
using System.Text.RegularExpressions;

namespace Heroes.Content.Definitions.Common
{
    public static class DefinitionKeyUtility
    {
        private static readonly Regex InvalidChars = new("[^a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string BuildKey(string typeKey, string displayName, string suffix)
        {
            var name = string.IsNullOrWhiteSpace(displayName)
                ? "new"
                : displayName.Trim().ToLower(CultureInfo.InvariantCulture);

            name = InvalidChars.Replace(name, "_");
            name = name.Trim('_');

            if (string.IsNullOrWhiteSpace(typeKey))
            {
                typeKey = "content";
            }

            return $"{typeKey}_{name}.{suffix}";
        }
    }
}
