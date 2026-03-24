using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class WaitNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();

            context.AddInputPort("Duration")
                .WithDataType<float>()
                .WithDisplayName("Duration")
                .WithDefaultValue(1f)
                .Build();

            context.AddInputPort("In")
                .WithDataType<GraphActionImplementationStepDefintion>()
                .WithDisplayName("In")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("Out")
                .WithDataType<GraphActionImplementationStepDefintion>()
                .WithDisplayName("Out")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
