using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class SetBeliefValueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();

            context.AddInputPort("Belief")
                .WithDataType<GraphBeliefRef>()
                .WithDisplayName("Belief")
                .Build();

            context.AddInputPort("Value")
                .WithDataType<float>()
                .WithDisplayName("Value")
                .Build();

            context.AddInputPort("In")
                .WithDataType<GraphActionEffectDefintion>()
                .WithDisplayName("In")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("Out")
                .WithDataType<GraphActionEffectDefintion>()
                .WithDisplayName("Out")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
