using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class AbsVector2Node : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<Vector2>()
                .WithDisplayName("Value")
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<Vector2>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}
