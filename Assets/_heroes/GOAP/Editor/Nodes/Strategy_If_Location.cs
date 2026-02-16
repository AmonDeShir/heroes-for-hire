using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_If_Location : StrategyGraphNode_Base
    {
        public const string OutTruePortName = "True";
        public const string OutFalsePortName = "False";

        public const string OptionOperator = "Operator";
        public const string ValueAPortName = "A";
        public const string ValueBPortName = "B";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapConditionOp>(OptionOperator)
                .WithDisplayName("Operator")
                .WithDefaultValue(GoapConditionOp.Equal);
        }

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

            var valueAPort = context.AddInputPort<LocationSO>(ValueAPortName)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var valueBPort = context.AddInputPort<LocationSO>(ValueBPortName)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valueAPort);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valueBPort);
        }
    }
}
