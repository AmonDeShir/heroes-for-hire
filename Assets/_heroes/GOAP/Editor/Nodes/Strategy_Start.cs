using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_Start : StrategyGraphNode_Base
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(OutPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}
