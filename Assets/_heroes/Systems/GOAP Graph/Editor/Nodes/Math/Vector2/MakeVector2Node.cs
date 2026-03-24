using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class MakeVector2Node : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("X")
                .WithDataType<float>()
                .WithDisplayName("X")
                .WithDefaultValue(0f)
                .Build();

            context.AddInputPort("Y")
                .WithDataType<float>()
                .WithDisplayName("Y")
                .WithDefaultValue(0f)
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<Vector2>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}
