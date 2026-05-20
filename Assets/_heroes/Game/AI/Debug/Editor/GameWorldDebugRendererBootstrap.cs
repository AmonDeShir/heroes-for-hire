using Heroes.GOAP.Editor;
using UnityEditor;

namespace Heroes.Game.AI.Debug
{
    [InitializeOnLoad]
    public static class GameWorldDebugRendererBootstrap
    {
        static GameWorldDebugRendererBootstrap()
        {
            GoapWorldDebugRendererRegistry.Register(new GameWorldDebugRenderer());
        }
    }
}


