using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class ImportanceResultNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Value")
                .WithDataType<float>()
                .WithDisplayName("Value")
                .Build();
            
            context.AddInputPort("Definition")
                .WithDataType<GraphGoalImportanceDefintion>()
                .WithDisplayName("Definition")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
