using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class AndNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("A")
                .WithDataType<bool>()
                .WithDisplayName("A")
                .Build();
            
            context.AddInputPort("B")
                .WithDataType<bool>()
                .WithDisplayName("B")
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<bool>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}