using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Wait : StrategyGraphNode_Base
    {
        public const string DurationPortName = "Duration";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var durationPort = context.AddInputPort<float>(DurationPortName)
                .WithDisplayName("Duration")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(durationPort);
        }
    }
}
