using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class MaxFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("A")
                .WithDataType<float>()
                .WithDefaultValue(0f)
                .Build();
            context.AddInputPort("B")
                .WithDataType<float>()
                .WithDefaultValue(0f)
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<float>()
                .Build();
        }
    }
}
