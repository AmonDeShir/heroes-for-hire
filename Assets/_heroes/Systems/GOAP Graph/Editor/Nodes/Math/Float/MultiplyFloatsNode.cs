using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class MultiplyFloatsNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("A")
                .WithDataType<float>()
                .WithDisplayName("A")
                .WithDefaultValue(0f)
                .Build();
            
            context.AddInputPort("B")
                .WithDataType<float>()
                .WithDisplayName("B")
                .WithDefaultValue(0f)
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}