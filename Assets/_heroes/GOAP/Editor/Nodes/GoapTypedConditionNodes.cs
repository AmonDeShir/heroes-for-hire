using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal abstract class Condition_Base : GraphNode_Base
    {
        public const string OutputPortName = "Condition";
        public const string ValuePortName = "Value";
        public const string VariablePortName = "Variable";
        public const string OptionOperator = "Operator";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapConditionOp>(OptionOperator)
                .WithDisplayName("Operator")
                .WithDefaultValue(GoapConditionOp.Equal);
        }

        protected void DefineCommonPorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapConditionNode>(OutputPortName)
                .WithDisplayName("Condition")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
        }
    }

    [Serializable]
    internal class Condition_Float : Condition_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<float>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

        }
    }

    [Serializable]
    internal class Condition_Bool : Condition_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<bool>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

        }
    }

    [Serializable]
    internal class Condition_Location : Condition_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<LocationSO>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

        }
    }
}
