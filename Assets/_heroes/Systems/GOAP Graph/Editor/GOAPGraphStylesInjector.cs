using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.Systems.GOAPGraph.Editor
{
    [InitializeOnLoad]
    internal static class GoapGraphStylesInjector
    {
        private const string StylesheetPath = "Assets/_heroes/Systems/GOAP Graph/Editor/Styles/CustomIcons.uss";
        private const string GraphWindowTypeName = "Unity.GraphToolkit.Editor.Implementation.GraphViewEditorWindowImp";
        private const string ItemLibraryWindowTypeName = "Unity.GraphToolkit.ItemLibrary.Editor.ItemLibraryWindow";

        private static readonly HashSet<int> InjectedWindowIds = new HashSet<int>();
        private static readonly HashSet<int> InjectedItemLibraryIds = new HashSet<int>();
        private static StyleSheet _customIconsStylesheet;
        private static double _lastUpdateTime;

        static GoapGraphStylesInjector()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime < 1.0d)
            {
                return;
            }

            _lastUpdateTime = EditorApplication.timeSinceStartup;

            if (_customIconsStylesheet == null)
            {
                _customIconsStylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylesheetPath);
                if (_customIconsStylesheet == null)
                {
                    return;
                }
            }

            var currentWindowIds = new HashSet<int>();
            var currentItemLibraryIds = new HashSet<int>();
            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            
            foreach (var window in windows)
            {
                if (window == null)
                {
                    continue;
                }

                var windowType = window.GetType();
                var fullName = windowType.FullName;

                if (string.Equals(fullName, GraphWindowTypeName, StringComparison.Ordinal))
                {
                    var instanceId = window.GetInstanceID();
                    currentWindowIds.Add(instanceId);

                    if (InjectedWindowIds.Contains(instanceId))
                    {
                        continue;
                    }

                    var root = GetBaseRootVisualElement(window);
                    if (root == null)
                    {
                        continue;
                    }

                    if (!root.styleSheets.Contains(_customIconsStylesheet))
                    {
                        root.styleSheets.Add(_customIconsStylesheet);
                    }

                    InjectedWindowIds.Add(instanceId);
                }
                else if (string.Equals(fullName, ItemLibraryWindowTypeName, StringComparison.Ordinal))
                {
                    var instanceId = window.GetInstanceID();
                    currentItemLibraryIds.Add(instanceId);

                    if (InjectedItemLibraryIds.Contains(instanceId))
                    {
                        continue;
                    }

                    var root = window.rootVisualElement;
                    if (root == null)
                    {
                        continue;
                    }

                    if (!root.styleSheets.Contains(_customIconsStylesheet))
                    {
                        root.styleSheets.Add(_customIconsStylesheet);
                    }

                    InjectedItemLibraryIds.Add(instanceId);
                }
            }

            InjectedWindowIds.IntersectWith(currentWindowIds);
            InjectedItemLibraryIds.IntersectWith(currentItemLibraryIds);
        }

        private static VisualElement GetBaseRootVisualElement(EditorWindow window)
        {
            if (window == null)
            {
                return null;
            }

            var root = window.rootVisualElement;
            if (root == null)
            {
                return null;
            }

            while (root.parent is { parent: not null })
            {
                root = root.parent;
            }

            return root;
        }
    }
}
