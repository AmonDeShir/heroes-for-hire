using System.IO;
using Heroes.Content.Definitions.Common;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Heroes.Editor.ContentEditor
{
    public static class InitProjectStructure
    {
        [MenuItem("Tools/Heroes/Init Project Structure")]
        public static void Run()
        {
            EnsureFolder(ContentEditorPaths.BuildingsFolder);
            EnsureFolder(ContentEditorPaths.UpgradesFolder);
            EnsureFolder(ContentEditorPaths.EntitiesFolder);
            EnsureFolder(ContentEditorPaths.HeroesFolder);
            EnsureFolder(ContentEditorPaths.ItemsFolder);
            EnsureFolder(ContentEditorPaths.SkillsFolder);
            EnsureFolder(ContentEditorPaths.EffectsFolder);
            EnsureFolder("Assets/Resources/Buildings");
            EnsureFolder(ContentEditorPaths.IconsFolder);

            EnsureLocalization();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        private static void EnsureLocalization()
        {
            if (LocalizationSettings.AvailableLocales == null || LocalizationSettings.AvailableLocales.Locales.Count == 0)
            {
                var locale = FindLocaleAsset("en") ?? CreateLocaleAsset("English", "en");
                if (locale != null)
                {
                    LocalizationEditorSettings.AddLocale(locale);
                    LocalizationSettings.ProjectLocale = locale;
                }
            }

            var collection = LocalizationEditorSettings.GetStringTableCollection("Definitions");
            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection("Definitions", "Assets/_heroes/Content/Localization");
            }

            if (collection != null)
            {
                var localeIdentifier = LocalizationSettings.ProjectLocale != null
                    ? LocalizationSettings.ProjectLocale.Identifier
                    : LocalizationSettings.AvailableLocales.Locales[0].Identifier;

                collection.GetTable(localeIdentifier);
            }

            CleanupDuplicateLocales();
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

        private static void CleanupDuplicateLocales()
        {
            var availableLocales = LocalizationSettings.AvailableLocales;
            if (availableLocales == null)
            {
                return;
            }

            var seen = new System.Collections.Generic.HashSet<string>();
            var locales = new System.Collections.Generic.List<Locale>(availableLocales.Locales);
            foreach (var locale in locales)
            {
                if (locale == null)
                {
                    continue;
                }

                var code = locale.Identifier.Code;
                if (seen.Contains(code))
                {
                    LocalizationEditorSettings.RemoveLocale(locale);
                }
                else
                {
                    seen.Add(code);
                }
            }
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
    }
}
