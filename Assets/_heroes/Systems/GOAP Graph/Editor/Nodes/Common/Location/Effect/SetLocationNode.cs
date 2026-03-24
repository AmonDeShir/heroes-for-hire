using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class SetLocationNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();

            context.AddInputPort("Location")
                .WithDataType<Vector2>()
                .WithDisplayName("Location")
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
