using System;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Condition_And : GraphNode_Base
    {
        public const string OutputPortName = "Condition";
        public const string InputPortName = "Conditions";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapConditionNode>(OutputPortName)
                .WithDisplayName("Condition")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var inputPort = context.AddInputPort<GoapConditionNode>(InputPortName)
                .WithDisplayName("Conditions")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetMulti(inputPort);
        }
    }

    [Serializable]
    internal class Condition_Or : GraphNode_Base
    {
        public const string OutputPortName = "Condition";
        public const string InputPortName = "Conditions";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapConditionNode>(OutputPortName)
                .WithDisplayName("Condition")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var inputPort = context.AddInputPort<GoapConditionNode>(InputPortName)
                .WithDisplayName("Conditions")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetMulti(inputPort);
        }
    }

    [Serializable]
    internal class Condition_Not : GraphNode_Base
    {
        public const string OutputPortName = "Condition";
        public const string InputPortName = "Condition";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapConditionNode>(OutputPortName)
                .WithDisplayName("Condition")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddInputPort<GoapConditionNode>(InputPortName)
                .WithDisplayName("Condition")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}
