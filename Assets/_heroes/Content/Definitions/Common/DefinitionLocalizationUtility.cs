using System.IO;
using Heroes.Content.Abstractions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
#endif

namespace Heroes.Content.Definitions.Common
{
    public static class DefinitionLocalizationUtility
    {
        private const string LocalizationFolder = "Assets/_heroes/Content/Localization";
        private const string TableCollectionName = "Definitions";

        public static void EnsureLocalizationEntries(DefinitionBase definition)
        {
#if UNITY_EDITOR
            if (definition == null)
            {
                return;
            }

            var id = definition.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var displayKey = definition.DisplayNameKey;
            var descriptionKey = definition.DescriptionKey;
            var typeKey = GetTypeKey(definition);

            if (string.IsNullOrWhiteSpace(displayKey))
            {
                displayKey = DefinitionKeyUtility.BuildKey(typeKey, definition.DisplayName, "name");
            }

            if (string.IsNullOrWhiteSpace(descriptionKey))
            {
                descriptionKey = DefinitionKeyUtility.BuildKey(typeKey, definition.DisplayName, "description");
            }

            EnsureFolder(LocalizationFolder);
            var collection = LocalizationEditorSettings.GetStringTableCollection(TableCollectionName);
            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection(TableCollectionName, LocalizationFolder);
            }

            if (collection == null)
            {
                return;
            }

            var availableLocales = LocalizationSettings.AvailableLocales;
            if (availableLocales == null || availableLocales.Locales.Count == 0)
            {
                var locale = FindLocaleAsset("en") ?? CreateLocaleAsset("English", "en");
                if (locale != null)
                {
                    LocalizationEditorSettings.AddLocale(locale);
                    LocalizationSettings.ProjectLocale = locale;
                }
            }

            var projectLocale = LocalizationSettings.ProjectLocale;
            var localeIdentifier = projectLocale != null
                ? projectLocale.Identifier
                : LocalizationSettings.AvailableLocales.Locales[0].Identifier;

            var stringTable = collection.GetTable(localeIdentifier) as StringTable;
            if (stringTable == null)
            {
                stringTable = collection.AddNewTable(localeIdentifier) as StringTable;
            }

            if (stringTable == null)
            {
                return;
            }

            var displayEntry = stringTable.GetEntry(displayKey) ?? stringTable.AddEntry(displayKey, definition.DisplayName);
            if (displayEntry != null && string.IsNullOrWhiteSpace(displayEntry.Value))
            {
                displayEntry.Value = definition.DisplayName;
            }

            var descriptionEntry = stringTable.GetEntry(descriptionKey) ?? stringTable.AddEntry(descriptionKey, definition.Description);
            if (descriptionEntry != null && string.IsNullOrWhiteSpace(descriptionEntry.Value))
            {
                descriptionEntry.Value = definition.Description;
            }

            var so = new SerializedObject(definition);
            so.FindProperty("displayNameKey").stringValue = displayKey;
            so.FindProperty("descriptionKey").stringValue = descriptionKey;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(stringTable);
#endif
        }

#if UNITY_EDITOR
        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        private static Locale CreateLocaleAsset(string displayName, string code)
        {
            var localeFolder = "Assets/_heroes/Content/Localization/Locales";
            EnsureFolder(localeFolder);

            var locale = Locale.CreateLocale(code);
            locale.name = displayName;

            var path = AssetDatabase.GenerateUniqueAssetPath($"{localeFolder}/{displayName}.asset");
            AssetDatabase.CreateAsset(locale, path);
            AssetDatabase.SaveAssets();

            return locale;
        }

        private static Locale FindLocaleAsset(string code)
        {
            var guids = AssetDatabase.FindAssets("t:Locale");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var locale = AssetDatabase.LoadAssetAtPath<Locale>(path);
                if (locale != null && locale.Identifier.Code == code)
                {
                    return locale;
                }
            }

            return null;
        }

        private static string GetTypeKey(DefinitionBase definition)
        {
            var type = definition.GetType();
            var name = type.Name;
            if (name.EndsWith("Definition"))
            {
                name = name.Substring(0, name.Length - "Definition".Length);
            }

            return name.ToLowerInvariant();
        }
#endif
    }
}
