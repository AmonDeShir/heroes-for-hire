using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class PredicateNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort("Predicate")
                .WithDataType<bool>()
                .WithDisplayName("Predicate")
                .Build();
            
            context.AddInputPort("True")
                .WithDataType<float>()
                .WithDisplayName("True")
                .Build();
            
            context.AddInputPort("False")
                .WithDataType<float>()
                .WithDisplayName("False")
                .Build();
            
            context.AddOutputPort("Result")
                .WithDataType<float>()
                .WithDisplayName("Result")
                .Build();
        }
    }
}