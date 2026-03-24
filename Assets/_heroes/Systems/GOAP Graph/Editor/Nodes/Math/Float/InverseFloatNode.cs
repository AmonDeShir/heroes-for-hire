using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class InverseFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<float>()
                .WithDefaultValue(1f)
                .Build();
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .Build();
        }
    }
}
