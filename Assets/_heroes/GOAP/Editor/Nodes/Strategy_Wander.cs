using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Wander : StrategyGraphNode_Base
    {
        public const string RadiusPortName = "Radius";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var radiusPort = context.AddInputPort<float>(RadiusPortName)
                .WithDisplayName("Radius")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(radiusPort);
        }
    }
}
