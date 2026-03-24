using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class BoolToFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Boolean")
                .WithDataType<bool>()
                .WithDisplayName("Boolean")
                .Build();
            
            context.AddOutputPort("Float")
                .WithDataType<float>()
                .WithDisplayName("Float")
                .Build();
        }
    }
}