using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Strategy_PlayAnimation : StrategyGraphNode_Base
    {
        public const string AnimationPortName = "Animation";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInOutPorts(context);

            var animationPort = context.AddInputPort<string>(AnimationPortName)
                .WithDisplayName("Animation")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(animationPort);
        }
    }
}
