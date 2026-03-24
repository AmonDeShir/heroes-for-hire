using System;
using Heroes.Systems.GOAPGraph.Editor;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class CompareVector2Node : Node
    {
        private const string OperatorOptionId = "operator";

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

            context.AddOutputPort("Value")
                .WithDataType<bool>()
                .WithDisplayName("Value")
                .Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);

            context.AddOption<CompareVector2Operator>(OperatorOptionId)
                .WithDisplayName("Operator")
                .WithDefaultValue(CompareVector2Operator.Equal)
                .Build();
        }
    }
}
