using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class SplitVector2Node : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<Vector2>()
                .WithDisplayName("Value")
                .Build();

            context.AddOutputPort("X")
                .WithDataType<float>()
                .WithDisplayName("X")
                .Build();

            context.AddOutputPort("Y")
                .WithDataType<float>()
                .WithDisplayName("Y")
                .Build();
        }
    }
}
