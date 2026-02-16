using System;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Action_Node : GraphNode_Base
    {
        public const string PreconditionsPortName = "Preconditions";
        public const string EffectsPortName = "Effects";
        public const string OutputPortName = "Action";
        public const string NamePortName = "Name";
        public const string CostPortName = "Cost";
        public const string StrategyPortName = "Strategy";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapActionDefinition>(OutputPortName)
                .WithDisplayName("Action")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var namePort = context.AddInputPort<string>(NamePortName)
                .WithDisplayName("Name")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();


            var costPort = context.AddInputPort<float>(CostPortName)
                .WithDisplayName("Cost")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();


            context.AddInputPort(StrategyPortName)
                .WithDisplayName("Strategy")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<GoapConditionNode>(PreconditionsPortName)
                .WithDisplayName("Preconditions")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddInputPort<GoapEffect>(EffectsPortName)
                .WithDisplayName("Effects")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

        }
    }
}
