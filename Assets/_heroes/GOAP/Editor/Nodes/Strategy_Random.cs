using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Random : StrategyGraphNode_Base
    {
        public const string OutAPortName = "A";
        public const string OutBPortName = "B";
        public const string ChancePortName = "ChanceA";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(InPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            var chancePort = context.AddInputPort<float>(ChancePortName)
                .WithDisplayName("Chance A")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort(OutAPortName)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(OutBPortName)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(chancePort);
        }
    }
}
