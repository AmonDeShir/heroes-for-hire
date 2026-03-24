using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class MaxVector2Node : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("A")
                .WithDataType<Vector2>()
                .WithDisplayName("A")
                .Build();

            context.AddInputPort("B")
                .WithDataType<Vector2>()
                .WithDisplayName("B")
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<Vector2>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}
