using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_SetValue_Float : StrategyGraphNode_Base
    {
        public const string VariablePortName = "Variable";
        public const string ValuePortName = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var valuePort = context.AddInputPort<float>(ValuePortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }
}
