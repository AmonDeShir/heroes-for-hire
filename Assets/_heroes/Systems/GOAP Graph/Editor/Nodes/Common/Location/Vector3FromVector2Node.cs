using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class Vector3FromVector2Node : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<Vector2>()
                .WithDisplayName("Value")
                .Build();

            context.AddInputPort("Y")
                .WithDataType<float>()
                .WithDisplayName("Y")
                .WithDefaultValue(0f)
                .Build();

            context.AddOutputPort("Result")
                .WithDataType<Vector3>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}
