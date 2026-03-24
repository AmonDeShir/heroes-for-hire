using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class LerpFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("t")
                .WithDataType<float>()
                .WithDisplayName("t")
                .WithDefaultValue(0.5f)
                .Build();
            
            context.AddInputPort("A")
                .WithDataType<float>()
                .WithDisplayName("A")
                .WithDefaultValue(0f)
                .Build();
            
            context.AddInputPort("B")
                .WithDataType<float>()
                .WithDisplayName("B")
                .WithDefaultValue(1f)
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}