using Heroes.GOAP.Editor;
using UnityEditor;

namespace GOAP.Demo.Debug
{
    [InitializeOnLoad]
    public static class DemoWorldDebugRendererBootstrap
    {
        static DemoWorldDebugRendererBootstrap()
        {
            GoapWorldDebugRendererRegistry.Register(new DemoWorldDebugRenderer());
        }
    }
}


