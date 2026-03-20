using System;
using UnityEngine;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class ArchetypeNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);
            
            context.AddInputPort("Beliefs")
                .WithDataType<GraphBelief>()
                .WithDisplayName("Beliefs")
                .Build()
                .WithMultiCapacity();
            
            context.AddOutputPort("Goals")
                .WithDataType<GraphGoal>()
                .WithDisplayName("Goals")
                .Build()
                .WithMultiCapacity();
            
            context.AddOutputPort("Actions")
                .WithDataType<GraphAction>()
                .WithDisplayName("Actions")
                .Build()
                .WithMultiCapacity();   
            
            context.AddOutputPort("Idle")
                .WithDataType<GraphIdleAction>()
                .WithDisplayName("Idle")
                .Build()
                .WithMultiCapacity();            
        }
    }
}
