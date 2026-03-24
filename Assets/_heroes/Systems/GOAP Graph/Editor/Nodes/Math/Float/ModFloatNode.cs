using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class ModuloNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            
            context.AddInputPort("Value")
                .WithDataType<float>()
                .WithDisplayName("Value")
                .WithDefaultValue(0f)
                .Build();
            
            context.AddInputPort("Modulo")
                .WithDataType<int>()
                .WithDisplayName("Modulo")
                .WithDefaultValue(1)
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}
