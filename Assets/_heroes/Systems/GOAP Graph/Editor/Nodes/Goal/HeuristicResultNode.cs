using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class HeuristicResultNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<float>()
                .WithDisplayName("Value")
                .Build();
            
            context.AddInputPort("Definition")
                .WithDataType<GraphGoalHeuristicDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
