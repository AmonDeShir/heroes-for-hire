using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class NegateNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<bool>()
                .WithDisplayName("Boolean")
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<bool>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}