using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class FloatToBoolNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Float")
                .WithDataType<float>()
                .WithDisplayName("Float")
                .Build();
            
            context.AddOutputPort("Boolean")
                .WithDataType<bool>()
                .WithDisplayName("Boolean")
                .Build();
        }
    }
}