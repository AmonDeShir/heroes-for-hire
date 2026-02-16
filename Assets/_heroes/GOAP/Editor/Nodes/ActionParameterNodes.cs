using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal abstract class ActionParameter_Base : GraphNode_Base
    {
        public const string OutputPortName = "Parameter";
        public const string ValuePortName = "Value";
        public const string VariablePortName = "Variable";

        protected void DefineCommonPorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapParameter>(OutputPortName)
                .WithDisplayName("Parameter")
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
    internal class ActionParameter_Float : ActionParameter_Base
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<float>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }

    [Serializable]
    internal class ActionParameter_Bool : ActionParameter_Base
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<bool>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }

    [Serializable]
    internal class ActionParameter_Location : ActionParameter_Base
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            DefineCommonPorts(context);
            var valuePort = context.AddInputPort<LocationSO>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }
}
