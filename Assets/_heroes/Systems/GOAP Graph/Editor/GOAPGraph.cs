using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Heroes.Systems.GOAPGraph.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class GOAPGraph : Graph
    {
        public const string AssetExtension = "goap";    
    
        [MenuItem("Assets/Create/GOAP Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GOAPGraph>();
        }
    }
}