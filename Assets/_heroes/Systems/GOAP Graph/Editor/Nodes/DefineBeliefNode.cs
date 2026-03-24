using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineBeliefNode : Node
    {
        public const string NameOptionId = "name";
        public const string DescriptionOptionId = "description";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddOutputPort("Belief")
                .WithDataType<GraphBelief>()
                .WithDisplayName("Belief")
                .Build();
                
            context.AddInputPort("Start Value")
                .WithDataType<float>()
                .WithDisplayName("Start Value")
                .WithDefaultValue(0f)
                .Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);

            context.AddOption<string>(NameOptionId)
                .WithDisplayName("Name")
                .WithDefaultValue(string.Empty)
                .Delayed()
                .Build();

            context.AddOption<string>(DescriptionOptionId)
                .WithDisplayName("Description")
                .WithDefaultValue(string.Empty)
                .Delayed()
                .Build();
        }
    }
}
