using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class DefineBeliefNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddOutputPort("Belief")
                .WithDataType<GraphBelief>()
                .WithDisplayName("Belief")
                .Build();

            context.AddInputPort("Name")
                .WithDataType<string>()
                .WithDisplayName("Name")
                .Build();
                
            context.AddInputPort("Description")
                .WithDataType<string>()
                .WithDisplayName("Description")
                .Build();
                
            context.AddInputPort("Start Value")
                .WithDataType<float>()
                .WithDisplayName("Start Value")
                .WithDefaultValue(0f)
                .Build();
        }
    }
}