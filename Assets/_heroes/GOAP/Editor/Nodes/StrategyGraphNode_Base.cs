using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal abstract class StrategyGraphNode_Base : Node
    {
        public const string InPortName = "In";
        public const string OutPortName = "Out";

        protected void AddInOutPorts(IPortDefinitionContext context)
        {
            context.AddInputPort(InPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(OutPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
