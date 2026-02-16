using System;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_AddValue : StrategyGraphNode_Base
    {
        public const string VariablePortName = "Variable";
        public const string DeltaPortName = "Delta";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var deltaPort = context.AddInputPort<float>(DeltaPortName)
                .WithDisplayName("Delta")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(deltaPort);
        }
    }
}
