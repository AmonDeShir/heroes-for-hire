using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class AchievedConditionNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Condition")
                .WithDataType<bool>()
                .WithDisplayName("Condition")
                .Build();

            context.AddInputPort("Definition")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("True")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("True")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("False")
                .WithDataType<GraphGoalAchievedDefintion>()
                .WithDisplayName("False")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
