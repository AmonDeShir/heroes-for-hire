using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Heroes.Content.Definitions.Buildings;
using Heroes.Content.Definitions.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Heroes.Editor.ContentEditor
{
    public static class ContentEditorUtility
    {
        private const string IconResourcesFolder = "Assets/Resources/Buildings";
        public static IEnumerable<DefinitionBase> LoadAssets<TBase>(string folderPath, Type assetType)
            where TBase : DefinitionBase
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return Array.Empty<DefinitionBase>();
            }

            var guids = AssetDatabase.FindAssets($"t:{assetType.Name}", new[] { folderPath });
            if (guids == null || guids.Length == 0)
            {
                return Array.Empty<DefinitionBase>();
            }

            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, assetType) as DefinitionBase)
                .Where(asset => asset != null)
                .OrderBy(asset => asset.DisplayName)
                .ToList();
        }

        public static DefinitionBase CreateAsset(Type assetType, string folderPath, string displayName, string idPrefix)
        {
            EnsureFolder(folderPath);
            var asset = ScriptableObject.CreateInstance(assetType) as DefinitionBase;
            if (asset == null)
            {
                return null;
            }

            var id = DefinitionIdUtility.GenerateId(displayName, idPrefix);
            var assetName = string.IsNullOrWhiteSpace(displayName) ? id : displayName;
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetName}.asset");
            AssetDatabase.CreateAsset(asset, path);

            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("displayName").stringValue = displayName ?? id;
            var typeKey = GetTypeKey(assetType, idPrefix);
            so.FindProperty("displayNameKey").stringValue = DefinitionKeyUtility.BuildKey(typeKey, displayName ?? id, "name");
            so.FindProperty("descriptionKey").stringValue = DefinitionKeyUtility.BuildKey(typeKey, displayName ?? id, "description");

            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.SaveAssets();
            return asset;
        }

        public static DefinitionBase CreateAssetForType(Type assetType, string displayName)
        {
            if (assetType == null)
            {
                return null;
            }

            if (!ContentEditorPaths.PathsByType.TryGetValue(assetType, out var folder))
            {
                return null;
            }

            var prefix = GetTypeKey(assetType, null);
            return CreateAsset(assetType, folder, displayName, prefix);
        }


        public static DefinitionBase DuplicateAsset(DefinitionBase asset, string folderPath)
        {
            if (asset == null)
            {
                return null;
            }

            var sourcePath = AssetDatabase.GetAssetPath(asset);
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var newPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{fileName}_Copy.asset");
            AssetDatabase.CopyAsset(sourcePath, newPath);
            var copy = AssetDatabase.LoadAssetAtPath<DefinitionBase>(newPath);
            return copy;
        }

        public static void DeleteAsset(DefinitionBase asset)
        {
            if (asset == null)
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        public static void RegenerateId(DefinitionBase asset, string prefix)
        {
            if (asset == null)
            {
                return;
            }

            var id = DefinitionIdUtility.GenerateId(asset.DisplayName, prefix);
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            var typeKey = GetTypeKey(asset.GetType(), prefix);
            so.FindProperty("displayNameKey").stringValue = DefinitionKeyUtility.BuildKey(typeKey, asset.DisplayName, "name");
            so.FindProperty("descriptionKey").stringValue = DefinitionKeyUtility.BuildKey(typeKey, asset.DisplayName, "description");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        public static Texture2D LoadIconFromResourcePath(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return null;
            }

            return Resources.Load<Texture2D>(resourcePath);
        }

        public static string GetResourcePathFromAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                return null;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            const string resourcesRoot = "Assets/Resources/";
            if (!path.StartsWith(resourcesRoot))
            {
                return null;
            }

            var resourcePath = path.Substring(resourcesRoot.Length);
            var extension = Path.GetExtension(resourcePath);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                resourcePath = resourcePath.Substring(0, resourcePath.Length - extension.Length);
            }

            return resourcePath;
        }

        public static void CaptureBuildingIcon(BuildingDefinition definition, Heroes.Tools.BuildingCreator.BuildingCreatorPreview preview, int size)
        {
            if (definition == null || preview == null)
            {
                return;
            }

            EnsureFolder(IconResourcesFolder);

            var fileName = string.IsNullOrWhiteSpace(definition.Id) ? definition.name : definition.Id;
            var assetPath = $"{IconResourcesFolder}/{fileName}.png";
            var resourcePath = $"Buildings/{fileName}";

            CaptureIcon(preview, assetPath, size);
            ConfigureIconTexture(assetPath);

            var so = new SerializedObject(definition);
            var iconPath = so.FindProperty("iconResourcePath");
            if (iconPath != null)
            {
                iconPath.stringValue = resourcePath;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(definition);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CaptureIconFromPreview(
            DefinitionBase definition,
            Heroes.Tools.BuildingCreator.BuildingCreatorPreview preview,
            int size,
            string iconFolder,
            string resourceRoot)
        {
            if (definition == null || preview == null)
            {
                return;
            }

            EnsureFolder(iconFolder);

            var fileName = string.IsNullOrWhiteSpace(definition.Id) ? definition.name : definition.Id;
            var assetPath = $"{iconFolder}/{fileName}.png";
            var resourcePath = string.IsNullOrWhiteSpace(resourceRoot)
                ? fileName
                : $"{resourceRoot}/{fileName}";

            CaptureIcon(preview, assetPath, size);
            ConfigureIconTexture(assetPath);

            var so = new SerializedObject(definition);
            var iconPath = so.FindProperty("iconResourcePath");
            if (iconPath != null)
            {
                iconPath.stringValue = resourcePath;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(definition);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CaptureIcon(Heroes.Tools.BuildingCreator.BuildingCreatorPreview preview, string outputPath, int size)
        {
            var camera = preview.CaptureCamera != null ? preview.CaptureCamera : Camera.main;
            if (camera == null)
            {
                return;
            }

            camera.enabled = true;
            camera.cullingMask = ~0;

            var previousTarget = camera.targetTexture;
            var previousClearFlags = camera.clearFlags;
            var previousColor = camera.backgroundColor;

            var rt = RenderTexture.GetTemporary(size, size, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = rt;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;

            GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
            camera.Render();
            RenderTexture.active = rt;

            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false, false);
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tex.Apply();

            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(outputPath, bytes);

            camera.targetTexture = previousTarget;
            camera.clearFlags = previousClearFlags;
            camera.backgroundColor = previousColor;
            RenderTexture.active = null;

            RenderTexture.ReleaseTemporary(rt);
            UnityEngine.Object.DestroyImmediate(tex);
        }

        private static void ConfigureIconTexture(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        private static string GetTypeKey(Type assetType, string fallback)
        {
            if (assetType == null)
            {
                return fallback ?? "content";
            }

            var name = assetType.Name;
            if (name.EndsWith("Definition"))
            {
                name = name.Substring(0, name.Length - "Definition".Length);
            }

            return name.ToLowerInvariant();
        }
    }
}
