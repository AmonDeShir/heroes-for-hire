using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineGoalNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Goal")
                .WithDataType<GraphGoal>()
                .WithDisplayName("Goal")
                .Build();

            context.AddInputPort("Name")
                .WithDataType<string>()
                .WithDisplayName("Name")
                .Build();

            context.AddInputPort("Description")
                .WithDataType<string>()
                .WithDisplayName("Description")
                .Build();

            context.AddInputPort("Priority")
                .WithDataType<int>()
                .WithDisplayName("Priority")
                .WithDefaultValue(1)
                .Build();
            
            context.AddOutputPort("Importance")
                .WithDataType<GraphGoalImportance>()
                .WithDisplayName("Importance")
                .Build();
            
            context.AddOutputPort("Achieved")
                .WithDataType<GraphGoalAchieved>()
                .WithDisplayName("Achieved")
                .Build();
            
            context.AddOutputPort("Heuristic")
                .WithDataType<GraphGoalHeuristic>()
                .WithDisplayName("Heuristic")
                .Build();
        }
    }
}
