using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal abstract class Effect_Base : GraphNode_Base
    {
        public const string OutputPortName = "Effect";
        public const string ValuePortName = "Value";
        public const string VariablePortName = "Variable";
        public const string OptionOperator = "Operator";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapEffectOp>(OptionOperator)
                .WithDisplayName("Operator")
                .WithDefaultValue(GoapEffectOp.Set);
        }

        protected void DefineCommonPorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapEffect>(OutputPortName)
                .WithDisplayName("Effect")
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
    internal class Effect_Float : Effect_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            context.AddInputPort<float>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .WithDefaultValue(0f)
                .Build();
        }
    }

    [Serializable]
    internal class Effect_Bool : Effect_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            context.AddInputPort<bool>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .WithDefaultValue(false)
                .Build();
        }
    }

    [Serializable]
    internal class Effect_Location : Effect_Base
    {

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            context.AddInputPort<LocationSO>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .WithDefaultValue(null)
                .Build();
        }
    }
}
