using System;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Goal_Node : GraphNode_Base
    {
        public const string DesiredPortName = "Desired";
        public const string OutputPortName = "Goal";
        public const string OptionName = "GoalName";
        public const string OptionPriority = "Priority";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(OptionName)
                .WithDisplayName("Name")
                .WithDefaultValue("Goal");

            context.AddOption<float>(OptionPriority)
                .WithDisplayName("Priority")
                .WithDefaultValue(1f);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapGoalDefinition>(OutputPortName)
                .WithDisplayName("Goal")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddInputPort<GoapConditionNode>(DesiredPortName)
                .WithDisplayName("Desired")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}
