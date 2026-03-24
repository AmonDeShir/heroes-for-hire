using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class NavSetDestinationNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();

            context.AddInputPort("Destination")
                .WithDataType<Vector2>()
                .WithDisplayName("Destination")
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
