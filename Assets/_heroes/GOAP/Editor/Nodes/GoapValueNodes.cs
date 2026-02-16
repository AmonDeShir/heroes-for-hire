using System;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Nodes
{
    [Serializable]
    internal class Value_Float : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string OptionValue = "Float";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<float>(OptionValue)
                .WithDisplayName("Value")
                .WithDefaultValue(0f);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<float>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class Value_Bool : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string OptionValue = "Bool";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<bool>(OptionValue)
                .WithDisplayName("Value")
                .WithDefaultValue(false);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<bool>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class Value_Location : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string OptionValue = "Location";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<LocationSO>(OptionValue)
                .WithDisplayName("Value")
                .WithDefaultValue(null);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<LocationSO>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class Value_String : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string OptionValue = "String";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(OptionValue)
                .WithDisplayName("Value")
                .WithDefaultValue(string.Empty);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<string>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class VariableRef_Node : GraphNode_Base
    {
        public const string OutputPortName = "Variable";
        public const string OptionValue = "Variable";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapVariableRef>(OptionValue)
                .WithDisplayName("Variable")
                .WithDefaultValue(new GoapVariableRef(string.Empty));
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<GoapVariableRef>(OutputPortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();
        }
    }

    [Serializable]
    internal class Value_FromVariable_Float : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string VariablePortName = "Variable";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<float>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
        }
    }

    [Serializable]
    internal class Value_FromVariable_Bool : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string VariablePortName = "Variable";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<bool>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
        }
    }

    [Serializable]
    internal class Value_FromVariable_Location : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string VariablePortName = "Variable";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var variablePort = context.AddInputPort<GoapVariableRef>(VariablePortName)
                .WithDisplayName("Variable")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<LocationSO>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(variablePort);
        }
    }

    [Serializable]
    internal class Value_FloatOp : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string InputAPortName = "A";
        public const string InputBPortName = "B";
        public const string OptionOperator = "Operator";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapFloatOp>(OptionOperator)
                .WithDisplayName("Operator")
                .WithDefaultValue(GoapFloatOp.Add);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var inputA = context.AddInputPort<float>(InputAPortName)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var inputB = context.AddInputPort<float>(InputBPortName)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<float>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(inputA);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(inputB);
        }
    }

    [Serializable]
    internal class Value_BoolOp : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string InputAPortName = "A";
        public const string InputBPortName = "B";
        public const string OptionOperator = "Operator";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GoapBoolOp>(OptionOperator)
                .WithDisplayName("Operator")
                .WithDefaultValue(GoapBoolOp.And);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var inputA = context.AddInputPort<bool>(InputAPortName)
                .WithDisplayName("A")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            var inputB = context.AddInputPort<bool>(InputBPortName)
                .WithDisplayName("B")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<bool>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(inputA);
            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(inputB);
        }
    }

    [Serializable]
    internal class Value_BoolNot : GraphNode_Base
    {
        public const string OutputPortName = "Value";
        public const string InputPortName = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            var input = context.AddInputPort<bool>(InputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            context.AddOutputPort<bool>(OutputPortName)
                .WithDisplayName("Value")
                .WithConnectorUI(PortConnectorUI.Circle)
                .Build();

            Heroes.Goap.Editor.Utilities.GoapPortCapacityHelper.SetNoEmbeddedConstant(input);
        }
    }
}
