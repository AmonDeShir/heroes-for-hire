using System;
using Heroes.Goap.Editor.Nodes;
using Heroes.Goap.Editor.Utilities;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Heroes.Goap.Editor.Graphs
{
    [Serializable]
    [Graph(AssetExtension)]
    [Subgraph(typeof(GoapArchetypeGraph))]
    internal class GoapStrategyGraph : Unity.GraphToolkit.Editor.Graph
    {
        internal const string AssetExtension = "goapstrat";

        [MenuItem("Assets/Create/Heroes/GOAP/Strategy Graph (Legacy)", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GoapStrategyGraph>("GOAP Strategy");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);
            ValidateStrategyVariables(graphLogger);
        }

        void ValidateStrategyVariables(GraphLogger graphLogger)
        {
            var variableTypes = Goap.Editor.Utilities.GoapGraphEditorContext.GetVariableTypesFromFocusedGraph();

            foreach (var node in GetNodes())
            {
                switch (node)
                {
                    case Strategy_SetValue_Float setFloat:
                        ValidateVariable(graphLogger, setFloat.GetInputPortByName(Strategy_SetValue_Float.VariablePortName), Heroes.Goap.Runtime.Values.GoapValueType.Float, variableTypes, setFloat);
                        break;
                    case Strategy_SetValue_Bool setBool:
                        ValidateVariable(graphLogger, setBool.GetInputPortByName(Strategy_SetValue_Bool.VariablePortName), Heroes.Goap.Runtime.Values.GoapValueType.Bool, variableTypes, setBool);
                        break;
                    case Strategy_SetValue_Location setLocation:
                        ValidateVariable(graphLogger, setLocation.GetInputPortByName(Strategy_SetValue_Location.VariablePortName), Heroes.Goap.Runtime.Values.GoapValueType.Location, variableTypes, setLocation);
                        break;
                    case Strategy_AddValue addValue:
                        ValidateVariable(graphLogger, addValue.GetInputPortByName(Strategy_AddValue.VariablePortName), Heroes.Goap.Runtime.Values.GoapValueType.Float, variableTypes, addValue);
                        break;
                }
            }
        }

        static void ValidateVariable(GraphLogger graphLogger, IPort port, Heroes.Goap.Runtime.Values.GoapValueType type, System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType> variableTypes, Node owner)
        {
            var variableRef = ReadVariableRefFromPort(port);
            if (string.IsNullOrWhiteSpace(variableRef.Name))
                return;

            if (!variableTypes.TryGetValue(variableRef.Name, out var actualType))
            {
                graphLogger.LogWarning("Strategy variable not found in any referencing GOAP graph.", owner);
                return;
            }

            if (actualType != type)
            {
                graphLogger.LogWarning("Strategy variable type mismatch.", owner);
            }
        }

        static Heroes.Goap.Runtime.Values.GoapVariableRef ReadVariableRefFromPort(IPort port)
        {
            if (port == null)
                return new Heroes.Goap.Runtime.Values.GoapVariableRef(string.Empty);

            var connected = new System.Collections.Generic.List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return new Heroes.Goap.Runtime.Values.GoapVariableRef(string.Empty);

            var node = connected[0].GetNode();
            if (node is Heroes.Goap.Editor.Nodes.VariableRef_Node variableNode)
                return GoapNodeOptionReader.GetOption(variableNode, Heroes.Goap.Editor.Nodes.VariableRef_Node.OptionValue, new Heroes.Goap.Runtime.Values.GoapVariableRef(string.Empty));

            return new Heroes.Goap.Runtime.Values.GoapVariableRef(string.Empty);
        }

    }
}
