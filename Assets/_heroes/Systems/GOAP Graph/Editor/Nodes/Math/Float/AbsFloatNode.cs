using System;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    [Serializable]
    public class AbsFloatNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("Value").WithDataType<float>().WithDefaultValue(0f).Build();
            context.AddOutputPort("Result").WithDataType<float>().Build();
        }
    }
}