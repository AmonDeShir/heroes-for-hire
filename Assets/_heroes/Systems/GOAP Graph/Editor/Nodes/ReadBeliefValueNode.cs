using System;
using Heroes.Systems.GOAPGraph.Editor;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class ReadBeliefValueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Belief")
                .WithDataType<GraphBeliefRef>()
                .WithDisplayName("Belief")
                .Build();

            context.AddInputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();
            
            context.AddOutputPort("Value")
                .WithDataType<float>()
                .WithDisplayName("Value")
                .Build();
        }
    }
}
