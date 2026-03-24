using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class GetWorldLocationNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();

            context.AddInputPort("LocationId")
                .WithDataType<GoapLocationId>()
                .WithDisplayName("LocationId")
                .Build();

            context.AddOutputPort("Location")
                .WithDataType<Vector2>()
                .WithDisplayName("Location")
                .Build();
        }
    }
}
