using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineHeuristicNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Heuristic")
                .WithDataType<GraphGoalHeuristic>()
                .WithDisplayName("Heuristic")
                .Build();
            
            context.AddOutputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();
            
            context.AddOutputPort("Definition")
                .WithDataType<GraphGoalHeuristicDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
