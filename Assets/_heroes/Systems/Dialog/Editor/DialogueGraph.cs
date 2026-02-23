using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

[Serializable]
[Graph(AssetExtension)]
public class DialogueGraph : Graph
{
    public const string AssetExtension = "dialouge";    
    
    [MenuItem("Assets/Create/Dialogue Graph")]
    private static void CreateAssetFile()
    {
        GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
    }
}
