using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineAchievedNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Achieved")
                .WithDataType<GraphGoalAchieved>()
                .WithDisplayName("Achieved")
                .Build();
            
            context.AddOutputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();
            
            context.AddOutputPort("Definition")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
