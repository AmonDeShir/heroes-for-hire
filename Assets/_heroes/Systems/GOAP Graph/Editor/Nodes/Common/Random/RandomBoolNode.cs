using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class RandomBoolNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Chance")
                .WithDataType<float>()
                .WithDisplayName("Chance")
                .WithDefaultValue(0.35f)
                .Build();

            context.AddOutputPort("Value")
                .WithDataType<bool>()
                .WithDisplayName("Value")
                .Build();
        }
    }
}
