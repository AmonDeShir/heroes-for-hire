using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineImportanceNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Importance")
                .WithDataType<GraphGoalImportance>()
                .WithDisplayName("Importance")
                .Build();
            
            context.AddOutputPort("Context")
                .WithDataType<GraphContext>()
                .WithDisplayName("Context")
                .Build();
            
            context.AddOutputPort("Definition")
                .WithDataType<GraphGoalImportanceDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
