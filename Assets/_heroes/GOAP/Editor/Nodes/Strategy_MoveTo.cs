using System;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_MoveTo : StrategyGraphNode_Base
    {
        public const string ValuePortName = "Location";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var valuePort = context.AddInputPort<LocationSO>(ValuePortName)
                .WithDisplayName("Location")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(valuePort);
        }
    }
}
