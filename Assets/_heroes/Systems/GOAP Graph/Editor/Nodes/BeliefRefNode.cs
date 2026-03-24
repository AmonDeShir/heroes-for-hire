using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class BeliefRefNode : Node
    {
        public const string BeliefOptionId = "Belief";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddOutputPort("Belief")
                .WithDataType<GraphBeliefRef>()
                .WithDisplayName("Belief")
                .Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);
            
            context.AddOption<GraphBeliefRef>(BeliefOptionId)
                .WithDisplayName("Belief")
                .Delayed()
                .Build();
        }
    }
}
