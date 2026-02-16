using System;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.World;
using Heroes.Goap.Editor.Utilities;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class ArchetypeRoot_Node : GraphNode_Base
    {
        public const string ActionsPortName = "Actions";
        public const string GoalsPortName = "Goals";
        const string TypeFloatPortName = "TypeFloat";
        const string TypeBoolPortName = "TypeBool";
        const string TypeLocationPortName = "TypeLocation";
        public const string OptionParent = "Parent";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapArchetypeAsset>(OptionParent)
                .WithDisplayName("Parent")
                .WithDefaultValue(null);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var actionsPort = context.AddInputPort<GoapActionDefinition>(ActionsPortName)
                .WithDisplayName("Actions")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            GoapPortCapacityHelper.SetMulti(actionsPort);

            var goalsPort = context.AddInputPort<GoapGoalDefinition>(GoalsPortName)
                .WithDisplayName("Goals")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            GoapPortCapacityHelper.SetMulti(goalsPort);

            context.AddOutputPort<float>(TypeFloatPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<bool>(TypeBoolPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<LocationSO>(TypeLocationPortName)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }
}
