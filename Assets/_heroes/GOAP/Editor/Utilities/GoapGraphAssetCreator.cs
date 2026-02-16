using System;
using System.Linq;
using System.Reflection;
using Heroes.Goap.Editor.Graphs;
using Heroes.Goap.Runtime.Strategies;
using UnityEditor;
using UnityEngine;

namespace Heroes.Goap.Editor.Utilities
{
    internal static class GoapGraphAssetCreator
    {
        public static GoapStrategyGraphAsset CreateStrategyGraphAsset(string folderPath, string baseName)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !folderPath.StartsWith("Assets"))
                folderPath = "Assets";

            var fileName = string.IsNullOrWhiteSpace(baseName) ? "Strategy" : baseName.Trim();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{fileName}.{GoapStrategyGraph.AssetExtension}");

            if (!CreateGraphObject(path))
                return null;

            AssetDatabase.ImportAsset(path);

            var runtime = AssetDatabase.LoadAssetAtPath<GoapStrategyGraphAsset>(path);
            if (runtime != null)
                return runtime;

            var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            return allAssets.OfType<GoapStrategyGraphAsset>().FirstOrDefault();
        }

        static bool CreateGraphObject(string assetPath)
        {
            var graphObjectType = FindType("Unity.GraphToolkit.Editor.Implementation.GraphObjectImp");
            var graphModelType = FindType("Unity.GraphToolkit.Editor.Implementation.GraphModelImp");
            var helpersType = FindType("Unity.GraphToolkit.Editor.GraphObjectCreationHelpers");
            if (graphObjectType == null || graphModelType == null || helpersType == null)
                return false;

            var createMethod = helpersType.GetMethod("CreateGraphObject", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (createMethod == null)
                return false;

            var fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            var parameters = new object[] { graphObjectType, graphModelType, fileName, assetPath, null };
            var result = createMethod.Invoke(null, parameters);
            return result != null;
        }

        static Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName, false);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
