using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class AchievedConditionalResultNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("ShouldReturn")
                .WithDataType<bool>()
                .WithDisplayName("ShouldReturn")
                .Build();

            context.AddInputPort("Value")
                .WithDataType<bool>()
                .WithDisplayName("Value")
                .Build();

            context.AddInputPort("Definition")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("Definition")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
