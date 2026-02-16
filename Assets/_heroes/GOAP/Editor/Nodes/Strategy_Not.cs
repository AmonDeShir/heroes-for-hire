using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Not : StrategyGraphNode_Base
    {
        public const string OutTruePortName = "True";
        public const string OutFalsePortName = "False";
        public const string ValuePortName = "Value";

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

            var valuePort = context.AddInputPort<bool>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }
}
