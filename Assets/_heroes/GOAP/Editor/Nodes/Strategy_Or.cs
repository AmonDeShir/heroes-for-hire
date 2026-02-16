using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Or : StrategyGraphNode_Base
    {
        public const string OutTruePortName = "True";
        public const string OutFalsePortName = "False";
        public const string ValueAPortName = "A";
        public const string ValueBPortName = "B";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(InPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(OutTruePortName)
                .WithDisplayName("True")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(OutFalsePortName)
                .WithDisplayName("False")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            var valueAPort = context.AddInputPort<bool>(ValueAPortName)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var valueBPort = context.AddInputPort<bool>(ValueBPortName)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valueAPort);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valueBPort);
        }
    }
}
