using System;
using System.Collections.Generic;
using System.Linq;
using Heroes.Goap.Editor.Nodes;
using Heroes.Goap.Editor.Utilities;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Heroes.Goap.Editor.Graphs
{
    [Serializable]
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    internal class GoapArchetypeGraph : Unity.GraphToolkit.Editor.Graph
    {
        internal const string AssetExtension = "goap";

        [MenuItem("Assets/Create/Heroes/GOAP/Archetype Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GoapArchetypeGraph>("GOAP Archetype");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);

            ValidateRoot(graphLogger, out var root);
            ValidateActions(graphLogger, root);
            ValidateGoals(graphLogger, root);
            ValidateConditions(graphLogger);
            ValidateEffects(graphLogger);
        }


        void ValidateRoot(GraphLogger graphLogger, out ArchetypeRoot_Node root)
        {
            var roots = GetNodes().OfType<ArchetypeRoot_Node>().ToList();
            root = roots.FirstOrDefault();

            if (roots.Count == 0)
            {
                graphLogger.LogError("Archetype graph requires one ArchetypeRoot_Node.");
                return;
            }

            if (roots.Count > 1)
            {
                graphLogger.LogError("Only one ArchetypeRoot_Node is allowed.", roots[1]);
            }
        }

        void ValidateActions(GraphLogger graphLogger, ArchetypeRoot_Node root)
        {
            if (root == null)
                return;

            var rootPort = root.GetInputPortByName(ArchetypeRoot_Node.ActionsPortName);
            var connected = new List<IPort>();
            rootPort?.GetConnectedPorts(connected);

            var connectedNodes = new HashSet<INode>(connected.Select(p => p.GetNode()).Where(n => n != null));
            var variableScopes = GetVariableScopeMap();
            var actionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var action in GetNodes().OfType<Action_Node>())
            {
                if (!connectedNodes.Contains(action))
                {
                    graphLogger.LogWarning("Action is not connected to Archetype Root.", action);
                }

                var namePort = action.GetInputPortByName(Action_Node.NamePortName);
                if (!TryGetStringFromPort(namePort, out var actionName, out var nameConnected))
                {
                    if (nameConnected)
                        graphLogger.LogWarning("Action Name must connect to Value_String.", action);
                    else
                        graphLogger.LogWarning("Action Name is empty.", action);
                }
                else if (!actionNames.Add(actionName))
                {
                    graphLogger.LogWarning("Action Name is duplicated.", action);
                }

                var strategyPort = action.GetInputPortByName(Action_Node.StrategyPortName);
                var strategyConnections = new List<IPort>();
                strategyPort?.GetConnectedPorts(strategyConnections);
                if (strategyConnections.Count == 0)
                {
                    graphLogger.LogWarning("Action has no Strategy Start connected.", action);
                }
                else
                {
                    var strategyNode = strategyConnections[0].GetNode();
                    var strategyStart = FindStrategyStart(strategyNode);
                    if (strategyStart == null)
                    {
                        graphLogger.LogWarning("Action Strategy must connect to Strategy_Start.", action);
                    }
                    else
                    {
                        WarnOnMissingStrategySets(graphLogger, action, strategyStart, variableScopes);
                    }
                }
            }
        }

        void ValidateGoals(GraphLogger graphLogger, ArchetypeRoot_Node root)
        {
            if (root == null)
                return;

            var rootPort = root.GetInputPortByName(ArchetypeRoot_Node.GoalsPortName);
            var connected = new List<IPort>();
            rootPort?.GetConnectedPorts(connected);

            var connectedNodes = new HashSet<INode>(connected.Select(p => p.GetNode()).Where(n => n != null));
            foreach (var goal in GetNodes().OfType<Goal_Node>())
            {
                if (!connectedNodes.Contains(goal))
                {
                    graphLogger.LogWarning("Goal is not connected to Archetype Root.", goal);
                }
            }
        }

        void ValidateConditions(GraphLogger graphLogger)
        {
            var variables = GetVariableMap();
            foreach (var condition in GetNodes().OfType<Condition_Base>())
            {
                var valuePort = condition.GetInputPortByName(Condition_Base.ValuePortName);
                if (valuePort != null)
                {
                    var valueConnections = new List<IPort>();
                    valuePort.GetConnectedPorts(valueConnections);
                    if (valueConnections.Count == 0 && !HasEmbeddedValue(valuePort, GetConditionValueType(condition)))
                        graphLogger.LogWarning("Condition Value is not connected.", condition);
                }

                if (!TryGetVariableRefFromPort(condition.GetInputPortByName(Condition_Base.VariablePortName), out var variableRef, out var hasConnection))
                {
                    if (hasConnection)
                        graphLogger.LogWarning("Condition variable must connect to VariableRef node.", condition);
                    continue;
                }

                var variableName = variableRef.Name;
                if (string.IsNullOrWhiteSpace(variableName))
                {
                    graphLogger.LogWarning("Condition has empty variable name.", condition);
                    continue;
                }

                if (!variables.TryGetValue(variableName, out var valueType))
                {
                    graphLogger.LogWarning("Condition references missing variable.", condition);
                    continue;
                }

                var nodeType = GetConditionValueType(condition);
                if (valueType != nodeType)
                {
                    graphLogger.LogWarning("Condition variable type mismatch.", condition);
                }
            }
        }

        void ValidateEffects(GraphLogger graphLogger)
        {
            var variables = GetVariableMap();
            foreach (var effect in GetNodes().OfType<Effect_Base>())
            {
                if (!TryGetVariableRefFromPort(effect.GetInputPortByName(Effect_Base.VariablePortName), out var variableRef, out var hasConnection))
                {
                    if (hasConnection)
                        graphLogger.LogWarning("Effect variable must connect to VariableRef node.", effect);
                    continue;
                }

                var variableName = variableRef.Name;
                if (string.IsNullOrWhiteSpace(variableName))
                {
                    graphLogger.LogWarning("Effect has empty variable name.", effect);
                    continue;
                }

                if (!variables.TryGetValue(variableName, out var valueType))
                {
                    graphLogger.LogWarning("Effect references missing variable.", effect);
                    continue;
                }

                var nodeType = GetEffectValueType(effect);
                if (valueType != nodeType)
                {
                    graphLogger.LogWarning("Effect variable type mismatch.", effect);
                }
            }
        }
        
        static GoapValueType GetConditionValueType(Condition_Base node)
        {
            if (node is Condition_Bool)
                return GoapValueType.Bool;
            if (node is Condition_Location)
                return GoapValueType.Location;
            return GoapValueType.Float;
        }

        static GoapValueType GetEffectValueType(Effect_Base node)
        {
            if (node is Effect_Bool)
                return GoapValueType.Bool;
            if (node is Effect_Location)
                return GoapValueType.Location;
            return GoapValueType.Float;
        }

        static GoapValueType GetParameterValueType(ActionParameter_Base node)
        {
            if (node is ActionParameter_Bool)
                return GoapValueType.Bool;
            if (node is ActionParameter_Location)
                return GoapValueType.Location;
            return GoapValueType.Float;
        }

        Dictionary<string, GoapValueType> GetVariableMap()
        {
            var map = new Dictionary<string, GoapValueType>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in GetVariables())
            {
                if (variable.dataType == typeof(float))
                    map[variable.name] = GoapValueType.Float;
                else if (variable.dataType == typeof(bool))
                    map[variable.name] = GoapValueType.Bool;
                else if (variable.dataType == typeof(LocationSO))
                    map[variable.name] = GoapValueType.Location;
            }

            return map;
        }

        Dictionary<string, GoapVariableScope> GetVariableScopeMap()
        {
            var map = new Dictionary<string, GoapVariableScope>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in GetVariables())
                map[variable.name] = variable.variableKind == VariableKind.Local ? GoapVariableScope.Memory : GoapVariableScope.World;

            return map;
        }

        static bool TryGetVariableRefFromPort(IPort port, out GoapVariableRef variableRef, out bool hasConnection)
        {
            variableRef = new GoapVariableRef(string.Empty);
            hasConnection = false;
            if (port == null)
                return false;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return false;

            hasConnection = true;

            var node = connected[0].GetNode();
            if (node is VariableRef_Node variableNode)
            {
                variableRef = GoapNodeOptionReader.GetOption(variableNode, VariableRef_Node.OptionValue, new GoapVariableRef(string.Empty));
                return true;
            }

            return false;
        }

        static bool TryGetStringFromPort(IPort port, out string value, out bool hasConnection)
        {
            value = string.Empty;
            hasConnection = false;
            if (port == null)
                return false;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
            {
                if (port.TryGetValue(out string embedded) && !string.IsNullOrWhiteSpace(embedded))
                {
                    value = embedded;
                    return true;
                }

                return false;
            }

            hasConnection = true;
            var node = connected[0].GetNode();
            if (node is Value_String stringNode)
            {
                value = GoapNodeOptionReader.GetOption(stringNode, Value_String.OptionValue, string.Empty);
                return !string.IsNullOrWhiteSpace(value);
            }

            return false;
        }

        static bool HasEmbeddedValue(IPort port, GoapValueType type)
        {
            if (port == null)
                return false;

            switch (type)
            {
                case GoapValueType.Float:
                    return port.TryGetValue(out float _);
                case GoapValueType.Bool:
                    return port.TryGetValue(out bool _);
                case GoapValueType.Location:
                    return port.TryGetValue(out LocationSO _);
                default:
                    return false;
            }
        }

        static Strategy_Start FindStrategyStart(INode node)
        {
            if (node == null)
                return null;

            if (node is Strategy_Start start)
                return start;

            var visited = new HashSet<INode>();
            var queue = new Queue<INode>();
            queue.Enqueue(node);
            visited.Add(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var input in current.GetInputPorts())
                {
                    var connected = new List<IPort>();
                    input.GetConnectedPorts(connected);
                    foreach (var connectedPort in connected)
                    {
                        var fromNode = connectedPort.GetNode();
                        if (fromNode == null || !visited.Add(fromNode))
                            continue;

                        if (fromNode is Strategy_Start found)
                            return found;

                        queue.Enqueue(fromNode);
                    }
                }
            }

            return null;
        }

        static List<StrategyGraphNode_Base> CollectStrategyNodes(Strategy_Start start)
        {
            var results = new List<StrategyGraphNode_Base>();
            var queue = new Queue<StrategyGraphNode_Base>();
            var visited = new HashSet<INode>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                results.Add(node);

                foreach (var output in node.GetOutputPorts())
                {
                    var connected = new List<IPort>();
                    output.GetConnectedPorts(connected);
                    foreach (var connectedPort in connected)
                    {
                        var next = connectedPort.GetNode();
                        if (next is StrategyGraphNode_Base strategyNode && visited.Add(strategyNode))
                            queue.Enqueue(strategyNode);
                    }
                }
            }

            return results;
        }

        void WarnOnMissingStrategySets(GraphLogger graphLogger, Action_Node action, Strategy_Start start, Dictionary<string, GoapVariableScope> variableScopes)
        {
            var strategyNodes = CollectStrategyNodes(start);
            var strategySets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < strategyNodes.Count; i++)
            {
                var node = strategyNodes[i];
                switch (node)
                {
                    case Strategy_SetValue_Float setFloat:
                        TryAddStrategyVariable(strategySets, setFloat.GetInputPortByName(Strategy_SetValue_Float.VariablePortName));
                        break;
                    case Strategy_SetValue_Bool setBool:
                        TryAddStrategyVariable(strategySets, setBool.GetInputPortByName(Strategy_SetValue_Bool.VariablePortName));
                        break;
                    case Strategy_SetValue_Location setLocation:
                        TryAddStrategyVariable(strategySets, setLocation.GetInputPortByName(Strategy_SetValue_Location.VariablePortName));
                        break;
                    case Strategy_AddValue addValue:
                        TryAddStrategyVariable(strategySets, addValue.GetInputPortByName(Strategy_AddValue.VariablePortName));
                        break;
                }
            }

            var effectsPort = action.GetInputPortByName(Action_Node.EffectsPortName);
            var connected = new List<IPort>();
            effectsPort?.GetConnectedPorts(connected);
            foreach (var connectedPort in connected)
            {
                var node = connectedPort.GetNode();
                if (!(node is Effect_Base effect))
                    continue;

                if (!TryGetVariableRefFromPort(effect.GetInputPortByName(Effect_Base.VariablePortName), out var variableRef, out var hasConnection))
                    continue;

                if (!hasConnection || string.IsNullOrWhiteSpace(variableRef.Name))
                    continue;

                if (variableScopes.TryGetValue(variableRef.Name, out var scope) && scope == GoapVariableScope.Memory)
                {
                    if (!strategySets.Contains(variableRef.Name))
                        graphLogger.LogWarning("Action effect updates Memory variable without Strategy_SetValue/AddValue.", action);
                }
            }
        }

        static void TryAddStrategyVariable(HashSet<string> set, IPort port)
        {
            if (!TryGetVariableRefFromPort(port, out var variableRef, out var hasConnection))
                return;

            if (hasConnection && !string.IsNullOrWhiteSpace(variableRef.Name))
                set.Add(variableRef.Name);
        }
    }

}
