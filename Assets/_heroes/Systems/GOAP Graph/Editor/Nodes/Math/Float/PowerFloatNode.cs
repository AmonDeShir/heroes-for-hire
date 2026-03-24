using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class PowerFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Base")
                .WithDataType<float>()
                .WithDefaultValue(1f)
                .Build();
            
            context.AddInputPort("Exponent")
                .WithDataType<float>()
                .WithDefaultValue(1f)
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<float>()
                .Build();
        }
    }
}
