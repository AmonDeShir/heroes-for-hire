using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class RandomFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Min")
                .WithDataType<float>()
                .WithDisplayName("Min")
                .WithDefaultValue(0f)
                .Build();
            
            context.AddInputPort("Max")
                .WithDataType<float>()
                .WithDisplayName("Max")
                .WithDefaultValue(1f)
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}