using System;
using Heroes.Systems.GOAPGraph.Editor;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class CompareFloatsNode : Node
    {
        private const string OperatorOptionId = "operator";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("A")
                .WithDataType<float>()
                .WithDisplayName("A")
                .Build();
            
            context.AddInputPort("B")
                .WithDataType<float>()
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

            context.AddOption<CompareFloatOperator>(OperatorOptionId)
                .WithDisplayName("Operator")
                .WithDefaultValue(CompareFloatOperator.Equal)
                .Build();
        }
    }
}
