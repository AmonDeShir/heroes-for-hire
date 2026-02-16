using System.Collections.Generic;
using System.Linq;
using Heroes.Goap.Editor.Graphs;
using Heroes.Goap.Editor.Nodes;
using Heroes.Goap.Editor.Utilities;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Strategies;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Heroes.Goap.Editor.Importers
{
    [ScriptedImporter(1, GoapArchetypeGraph.AssetExtension)]
    internal class GoapArchetypeGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<GoapArchetypeGraph>(ctx.assetPath);
            if (graph == null)
            {
                Debug.LogError($"Failed to load GOAP archetype graph asset: {ctx.assetPath}");
                return;
            }

            var root = graph.GetNodes().OfType<ArchetypeRoot_Node>().FirstOrDefault();
            if (root == null)
            {
                Debug.LogError("GOAP archetype graph requires an ArchetypeRoot_Node.");
                return;
            }

            var runtimeGraph = ScriptableObject.CreateInstance<GoapGraphAsset>();
            BuildVariables(graph, runtimeGraph);
            BuildActions(root, runtimeGraph, ctx);
            BuildGoals(root, runtimeGraph);

            var runtimeArchetype = ScriptableObject.CreateInstance<GoapArchetypeAsset>();
            runtimeArchetype.Graph = runtimeGraph;
            runtimeArchetype.Parent = GoapNodeOptionReader.GetOption<GoapArchetypeAsset>(root, ArchetypeRoot_Node.OptionParent, null);

            ctx.AddObjectToAsset("Archetype", runtimeArchetype);
            ctx.AddObjectToAsset("Graph", runtimeGraph);
            ctx.SetMainObject(runtimeArchetype);
        }

        static void BuildVariables(GoapArchetypeGraph graph, GoapGraphAsset runtime)
        {
            foreach (var variable in graph.GetVariables())
            {
                if (!TryResolveValueType(variable.dataType, out var valueType))
                    continue;

                var def = new GoapVariableDef
                {
                    Name = variable.name,
                    Type = valueType,
                    Scope = ResolveScope(variable.variableKind),
                    DefaultValue = ReadDefaultValue(variable, valueType)
                };

                runtime.Variables.Add(def);
            }
        }

        static void BuildActions(ArchetypeRoot_Node root, GoapGraphAsset runtime, AssetImportContext ctx)
        {
            var nodes = GetConnectedNodes(root.GetInputPortByName(ArchetypeRoot_Node.ActionsPortName))
                .OfType<Action_Node>();

            var actionIndex = 0;
            foreach (var node in nodes)
            {
                var name = ReadStringValue(node.GetInputPortByName(Action_Node.NamePortName), "Action");
                var cost = ReadPortValueStrict(node.GetInputPortByName(Action_Node.CostPortName), GoapValueType.Float, 1f);
                var action = new GoapActionDefinition
                {
                    Id = name,
                    Name = name,
                    BaseCost = cost.FloatValue,
                    Strategy = BuildStrategy(node, ctx, actionIndex, name)
                };

                action.Preconditions.AddRange(ReadConditions(node.GetInputPortByName(Action_Node.PreconditionsPortName)));
                action.Effects.AddRange(ReadEffects(node.GetInputPortByName(Action_Node.EffectsPortName)));

                runtime.Actions.Add(action);
                actionIndex++;
            }
        }

        static void BuildGoals(ArchetypeRoot_Node root, GoapGraphAsset runtime)
        {
            var nodes = GetConnectedNodes(root.GetInputPortByName(ArchetypeRoot_Node.GoalsPortName))
                .OfType<Goal_Node>();

            foreach (var node in nodes)
            {
                var name = GoapNodeOptionReader.GetOption(node, Goal_Node.OptionName, "Goal");
                var goal = new GoapGoalDefinition
                {
                    Id = name,
                    Name = name,
                    Priority = GoapNodeOptionReader.GetOption(node, Goal_Node.OptionPriority, 1f)
                };

                goal.Desired.AddRange(ReadConditions(node.GetInputPortByName(Goal_Node.DesiredPortName)));
                runtime.Goals.Add(goal);
            }
        }

        static IEnumerable<INode> GetConnectedNodes(IPort port)
        {
            if (port == null)
                return Enumerable.Empty<INode>();

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            return connected.Select(p => p.GetNode()).Where(n => n != null).Distinct();
        }

        static GoapStrategyGraphAsset BuildStrategy(Action_Node node, AssetImportContext ctx, int index, string actionName)
        {
            var start = ReadStrategyStart(node);
            if (start == null)
                return null;

            var strategyNodes = CollectStrategyNodes(start);
            if (strategyNodes.Count == 0)
                return null;

            var runtime = ScriptableObject.CreateInstance<GoapStrategyGraphAsset>();
            var nodeIds = new Dictionary<INode, int>();
            for (int i = 0; i < strategyNodes.Count; i++)
                nodeIds[strategyNodes[i]] = i;

            foreach (var strategyNode in strategyNodes)
            {
                if (strategyNode is Strategy_Start)
                {
                    runtime.EntryNodeId = nodeIds[strategyNode];
                    runtime.Nodes.Add(new GoapStartNode { Id = nodeIds[strategyNode] });
                }
                else if (strategyNode is Strategy_Wait waitNode)
                {
                    var duration = ReadPortValueStrict(waitNode.GetInputPortByName(Strategy_Wait.DurationPortName), GoapValueType.Float, 1f);
                    runtime.Nodes.Add(new GoapWaitNode
                    {
                        Id = nodeIds[strategyNode],
                        Duration = duration.FloatValue
                    });
                }
                else if (strategyNode is Strategy_SetValue_Float setNode)
                {
                    var variableRef = ReadVariableRef(setNode.GetInputPortByName(Strategy_SetValue_Float.VariablePortName));
                    var value = ReadPortValueStrict(setNode.GetInputPortByName(Strategy_SetValue_Float.ValuePortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[strategyNode],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (strategyNode is Strategy_SetValue_Bool setBool)
                {
                    var variableRef = ReadVariableRef(setBool.GetInputPortByName(Strategy_SetValue_Bool.VariablePortName));
                    var value = ReadPortValueStrict(setBool.GetInputPortByName(Strategy_SetValue_Bool.ValuePortName), GoapValueType.Bool, false);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[strategyNode],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (strategyNode is Strategy_SetValue_Location setLocation)
                {
                    var variableRef = ReadVariableRef(setLocation.GetInputPortByName(Strategy_SetValue_Location.VariablePortName));
                    var value = ReadPortValueStrict(setLocation.GetInputPortByName(Strategy_SetValue_Location.ValuePortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[strategyNode],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (strategyNode is Strategy_AddValue addNode)
                {
                    var variableRef = ReadVariableRef(addNode.GetInputPortByName(Strategy_AddValue.VariablePortName));
                    var delta = ReadPortValueStrict(addNode.GetInputPortByName(Strategy_AddValue.DeltaPortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapAddValueNode
                    {
                        Id = nodeIds[strategyNode],
                        VariableName = variableRef.Name,
                        Delta = delta.FloatValue
                    });
                }
                else if (strategyNode is Strategy_If_Float ifFloat)
                {
                    var left = ReadPortValueStrict(ifFloat.GetInputPortByName(Strategy_If_Float.ValueAPortName), GoapValueType.Float, 0f);
                    var right = ReadPortValueStrict(ifFloat.GetInputPortByName(Strategy_If_Float.ValueBPortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[strategyNode],
                        Operator = GoapNodeOptionReader.GetOption(ifFloat, Strategy_If_Float.OptionOperator, GoapConditionOp.GreaterOrEqual),
                        Left = left,
                        Right = right
                    });
                }
                else if (strategyNode is Strategy_If_Bool ifBool)
                {
                    var left = ReadPortValueStrict(ifBool.GetInputPortByName(Strategy_If_Bool.ValueAPortName), GoapValueType.Bool, false);
                    var right = ReadPortValueStrict(ifBool.GetInputPortByName(Strategy_If_Bool.ValueBPortName), GoapValueType.Bool, false);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[strategyNode],
                        Operator = GoapNodeOptionReader.GetOption(ifBool, Strategy_If_Bool.OptionOperator, GoapConditionOp.Equal),
                        Left = left,
                        Right = right
                    });
                }
                else if (strategyNode is Strategy_If_Location ifLocation)
                {
                    var left = ReadPortValueStrict(ifLocation.GetInputPortByName(Strategy_If_Location.ValueAPortName), GoapValueType.Location, null);
                    var right = ReadPortValueStrict(ifLocation.GetInputPortByName(Strategy_If_Location.ValueBPortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[strategyNode],
                        Operator = GoapNodeOptionReader.GetOption(ifLocation, Strategy_If_Location.OptionOperator, GoapConditionOp.Equal),
                        Left = left,
                        Right = right
                    });
                }
                else if (strategyNode is Strategy_Random randomNode)
                {
                    var chance = ReadPortValueStrict(randomNode.GetInputPortByName(Strategy_Random.ChancePortName), GoapValueType.Float, 0.5f);
                    runtime.Nodes.Add(new GoapRandomNode
                    {
                        Id = nodeIds[strategyNode],
                        ChanceA = chance.FloatValue
                    });
                }
                else if (strategyNode is Strategy_PlayAnimation animNode)
                {
                    var animation = ReadStringValue(animNode.GetInputPortByName(Strategy_PlayAnimation.AnimationPortName), string.Empty);
                    runtime.Nodes.Add(new GoapPlayAnimationNode
                    {
                        Id = nodeIds[strategyNode],
                        AnimationName = animation
                    });
                }
                else if (strategyNode is Strategy_MoveTo moveNode)
                {
                    var locationValue = ReadPortValueStrict(moveNode.GetInputPortByName(Strategy_MoveTo.ValuePortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapMoveToLocationNode
                    {
                        Id = nodeIds[strategyNode],
                        Target = locationValue.LocationValue
                    });
                }
                else if (strategyNode is Strategy_Wander wanderNode)
                {
                    var radius = ReadPortValueStrict(wanderNode.GetInputPortByName(Strategy_Wander.RadiusPortName), GoapValueType.Float, 5f);
                    runtime.Nodes.Add(new GoapWanderNode
                    {
                        Id = nodeIds[strategyNode],
                        Radius = radius.FloatValue
                    });
                }
            }

            foreach (var strategyNode in strategyNodes)
            {
                var outputs = strategyNode.GetOutputPorts();
                foreach (var output in outputs)
                {
                    var connected = new List<IPort>();
                    output.GetConnectedPorts(connected);
                    foreach (var connectedPort in connected)
                    {
                        var toNode = connectedPort.GetNode();
                        if (toNode == null || !nodeIds.ContainsKey(toNode))
                            continue;

                        runtime.Edges.Add(new GoapStrategyEdge
                        {
                            FromNodeId = nodeIds[strategyNode],
                            FromPortName = output.name,
                            ToNodeId = nodeIds[toNode]
                        });
                    }
                }
            }

            var safeName = string.IsNullOrWhiteSpace(actionName) ? "Strategy" : actionName;
            foreach (var invalid in System.IO.Path.GetInvalidFileNameChars())
                safeName = safeName.Replace(invalid, '_');

            ctx.AddObjectToAsset($"Strategy_{safeName}_{index}", runtime);
            return runtime;
        }

        static Strategy_Start ReadStrategyStart(Action_Node node)
        {
            var port = node.GetInputPortByName(Action_Node.StrategyPortName);
            if (port == null)
                return null;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return null;

            return FindStrategyStart(connected[0].GetNode());
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

        static List<GoapConditionNode> ReadConditions(IPort port)
        {
            var results = new List<GoapConditionNode>();
            if (port == null)
                return results;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            foreach (var connectedPort in connected)
            {
                var node = connectedPort.GetNode();
                var condition = TryReadConditionNode(node, new HashSet<INode>());
                if (condition != null)
                    results.Add(condition);
            }

            return results;
        }

        static List<GoapEffect> ReadEffects(IPort port)
        {
            var results = new List<GoapEffect>();
            if (port == null)
                return results;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            foreach (var connectedPort in connected)
            {
                var node = connectedPort.GetNode();
                var effect = TryReadEffect(node);
                if (effect.HasValue)
                    results.Add(effect.Value);
            }

            return results;
        }

        static GoapConditionNode TryReadConditionNode(INode node, HashSet<INode> visited)
        {
            if (node == null || visited.Contains(node))
                return null;

            visited.Add(node);
            switch (node)
            {
                case Condition_Float floatNode:
                    return BuildCondition(floatNode, ReadValueExpression(floatNode.GetInputPortByName(Condition_Base.ValuePortName), GoapValueType.Float));
                case Condition_Bool boolNode:
                    return BuildCondition(boolNode, ReadValueExpression(boolNode.GetInputPortByName(Condition_Base.ValuePortName), GoapValueType.Bool));
                case Condition_Location locationNode:
                    return BuildCondition(locationNode, ReadValueExpression(locationNode.GetInputPortByName(Condition_Base.ValuePortName), GoapValueType.Location));
                case Condition_And andNode:
                    return BuildConditionGroup(andNode.GetInputPortByName(Condition_And.InputPortName), visited, true);
                case Condition_Or orNode:
                    return BuildConditionGroup(orNode.GetInputPortByName(Condition_Or.InputPortName), visited, false);
                case Condition_Not notNode:
                    return BuildConditionNot(notNode.GetInputPortByName(Condition_Not.InputPortName), visited);
            }

            return null;
        }

        static GoapEffect? TryReadEffect(INode node)
        {
            switch (node)
            {
                case Effect_Float floatNode:
                    return BuildEffect(floatNode, ReadFloatValue(floatNode.GetInputPortByName(Effect_Base.ValuePortName)));
                case Effect_Bool boolNode:
                    return BuildEffect(boolNode, ReadBoolValue(boolNode.GetInputPortByName(Effect_Base.ValuePortName)));
                case Effect_Location locationNode:
                    return BuildEffect(locationNode, ReadLocationValue(locationNode.GetInputPortByName(Effect_Base.ValuePortName)));
            }

            return null;
        }
        
        static GoapConditionNode BuildCondition(Condition_Base node, GoapValueExpression rightExpression)
        {
            var variableRef = ReadVariableRef(node.GetInputPortByName(Condition_Base.VariablePortName));
            return new GoapConditionCompare
            {
                Operator = GoapNodeOptionReader.GetOption(node, Condition_Base.OptionOperator, GoapConditionOp.Equal),
                Left = new GoapValueVariable
                {
                    VariableName = variableRef.Name,
                    Type = GetConditionValueType(node)
                },
                Right = rightExpression
            };
        }

        static GoapEffect BuildEffect(Effect_Base node, GoapValue value)
        {
            var variableRef = ReadVariableRef(node.GetInputPortByName(Effect_Base.VariablePortName));
            return new GoapEffect
            {
                VariableName = variableRef.Name,
                Operator = GoapNodeOptionReader.GetOption(node, Effect_Base.OptionOperator, GoapEffectOp.Set),
                Value = value
            };
        }

        static GoapConditionNode BuildConditionGroup(IPort port, HashSet<INode> visited, bool isAnd)
        {
            var connected = new List<IPort>();
            port?.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return null;

            var list = new List<GoapConditionNode>();
            foreach (var connectedPort in connected)
            {
                var child = TryReadConditionNode(connectedPort.GetNode(), visited);
                if (child != null)
                    list.Add(child);
            }

            if (list.Count == 0)
                return null;

            if (isAnd)
                return new GoapConditionAnd { Conditions = list };

            return new GoapConditionOr { Conditions = list };
        }

        static GoapConditionNode BuildConditionNot(IPort port, HashSet<INode> visited)
        {
            var connected = new List<IPort>();
            port?.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return null;

            var child = TryReadConditionNode(connected[0].GetNode(), visited);
            if (child == null)
                return null;

            return new GoapConditionNot { Condition = child };
        }

        static GoapValueExpression ReadValueExpression(IPort port, GoapValueType type)
        {
            var connected = new List<IPort>();
            port?.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return new GoapValueConstant { Value = DefaultValue(type) };

            var visited = new HashSet<INode>();
            return BuildValueExpression(connected[0].GetNode(), type, visited) ?? new GoapValueConstant { Value = DefaultValue(type) };
        }

        static GoapValueExpression BuildValueExpression(INode node, GoapValueType type, HashSet<INode> visited)
        {
            if (node == null || visited.Contains(node))
                return null;

            visited.Add(node);

            switch (node)
            {
                case Value_Float floatNode when type == GoapValueType.Float:
                    return new GoapValueConstant
                    {
                        Value = GoapValue.FromFloat(GoapNodeOptionReader.GetOption(floatNode, Value_Float.OptionValue, 0f))
                    };
                case Value_Bool boolNode when type == GoapValueType.Bool:
                    return new GoapValueConstant
                    {
                        Value = GoapValue.FromBool(GoapNodeOptionReader.GetOption(boolNode, Value_Bool.OptionValue, false))
                    };
                case Value_Location locationNode when type == GoapValueType.Location:
                    return new GoapValueConstant
                    {
                        Value = GoapValue.FromLocation(GoapNodeOptionReader.GetOption<LocationSO>(locationNode, Value_Location.OptionValue, null))
                    };
                case Value_FromVariable_Float varFloat when type == GoapValueType.Float:
                    return new GoapValueVariable
                    {
                        VariableName = ReadVariableRef(varFloat.GetInputPortByName(Value_FromVariable_Float.VariablePortName)).Name,
                        Type = GoapValueType.Float
                    };
                case Value_FromVariable_Bool varBool when type == GoapValueType.Bool:
                    return new GoapValueVariable
                    {
                        VariableName = ReadVariableRef(varBool.GetInputPortByName(Value_FromVariable_Bool.VariablePortName)).Name,
                        Type = GoapValueType.Bool
                    };
                case Value_FromVariable_Location varLocation when type == GoapValueType.Location:
                    return new GoapValueVariable
                    {
                        VariableName = ReadVariableRef(varLocation.GetInputPortByName(Value_FromVariable_Location.VariablePortName)).Name,
                        Type = GoapValueType.Location
                    };
                case Value_FloatOp floatOp when type == GoapValueType.Float:
                    return new GoapFloatBinaryOp
                    {
                        Operator = GoapNodeOptionReader.GetOption(floatOp, Value_FloatOp.OptionOperator, GoapFloatOp.Add),
                        A = ReadValueExpression(floatOp.GetInputPortByName(Value_FloatOp.InputAPortName), GoapValueType.Float),
                        B = ReadValueExpression(floatOp.GetInputPortByName(Value_FloatOp.InputBPortName), GoapValueType.Float)
                    };
                case Value_BoolOp boolOp when type == GoapValueType.Bool:
                    return new GoapBoolBinaryOp
                    {
                        Operator = GoapNodeOptionReader.GetOption(boolOp, Value_BoolOp.OptionOperator, GoapBoolOp.And),
                        A = ReadValueExpression(boolOp.GetInputPortByName(Value_BoolOp.InputAPortName), GoapValueType.Bool),
                        B = ReadValueExpression(boolOp.GetInputPortByName(Value_BoolOp.InputBPortName), GoapValueType.Bool)
                    };
                case Value_BoolNot boolNot when type == GoapValueType.Bool:
                    return new GoapBoolNot
                    {
                        Value = ReadValueExpression(boolNot.GetInputPortByName(Value_BoolNot.InputPortName), GoapValueType.Bool)
                    };
            }

            return null;
        }

        static GoapValue DefaultValue(GoapValueType type)
        {
            return type switch
            {
                GoapValueType.Float => GoapValue.FromFloat(0f),
                GoapValueType.Bool => GoapValue.FromBool(false),
                GoapValueType.Location => GoapValue.FromLocation(null),
                _ => default
            };
        }

        static GoapValueType GetConditionValueType(Condition_Base node)
        {
            if (node is Condition_Bool)
                return GoapValueType.Bool;
            if (node is Condition_Location)
                return GoapValueType.Location;
            return GoapValueType.Float;
        }

        static GoapValue ReadFloatValue(IPort port)
        {
            if (TryReadConnectedValue(port, out float connected))
                return GoapValue.FromFloat(connected);
            if (port != null && port.TryGetValue(out float embedded))
                return GoapValue.FromFloat(embedded);
            return GoapValue.FromFloat(0f);
        }

        static GoapValue ReadBoolValue(IPort port)
        {
            if (TryReadConnectedValue(port, out bool connected))
                return GoapValue.FromBool(connected);
            if (port != null && port.TryGetValue(out bool embedded))
                return GoapValue.FromBool(embedded);
            return GoapValue.FromBool(false);
        }

        static GoapValue ReadLocationValue(IPort port)
        {
            if (TryReadConnectedValue(port, out LocationSO connected))
                return GoapValue.FromLocation(connected);
            if (port != null && port.TryGetValue(out LocationSO embedded))
                return GoapValue.FromLocation(embedded);
            return GoapValue.FromLocation(null);
        }

        static GoapValue ReadPortValueStrict(IPort port, GoapValueType valueType, object fallback)
        {
            switch (valueType)
            {
                case GoapValueType.Float:
                    if (TryReadConnectedValue(port, out float floatValue))
                        return GoapValue.FromFloat(floatValue);
                    break;
                case GoapValueType.Bool:
                    if (TryReadConnectedValue(port, out bool boolValue))
                        return GoapValue.FromBool(boolValue);
                    break;
                case GoapValueType.Location:
                    if (TryReadConnectedValue(port, out LocationSO locationValue))
                        return GoapValue.FromLocation(locationValue);
                    break;
            }

            return valueType switch
            {
                GoapValueType.Float => GoapValue.FromFloat(fallback is float f ? f : 0f),
                GoapValueType.Bool => GoapValue.FromBool(fallback is bool b && b),
                GoapValueType.Location => GoapValue.FromLocation(fallback as LocationSO),
                _ => default
            };
        }

        static string ReadStringValue(IPort port, string fallback)
        {
            if (port != null)
            {
                var connected = new List<IPort>();
                port.GetConnectedPorts(connected);
                if (connected.Count > 0)
                {
                    var node = connected[0].GetNode();
                    if (node is Value_String stringNode)
                        return GoapNodeOptionReader.GetOption(stringNode, Value_String.OptionValue, fallback);
                }
            }

            return fallback;
        }

        static GoapVariableRef ReadVariableRef(IPort port)
        {
            if (port == null)
                return new GoapVariableRef(string.Empty);

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return new GoapVariableRef(string.Empty);

            var node = connected[0].GetNode();
            if (node is VariableRef_Node variableNode)
                return GoapNodeOptionReader.GetOption(variableNode, VariableRef_Node.OptionValue, new GoapVariableRef(string.Empty));

            return new GoapVariableRef(string.Empty);
        }

        static bool TryReadConnectedValue<T>(IPort port, out T value)
        {
            value = default;
            if (port == null)
                return false;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return false;

            var node = connected[0].GetNode();
            if (node is Value_Float floatNode && typeof(T) == typeof(float))
            {
                value = (T)(object)GoapNodeOptionReader.GetOption(floatNode, Value_Float.OptionValue, 0f);
                return true;
            }
            if (node is Value_Bool boolNode && typeof(T) == typeof(bool))
            {
                value = (T)(object)GoapNodeOptionReader.GetOption(boolNode, Value_Bool.OptionValue, false);
                return true;
            }
            if (node is Value_Location locationNode && typeof(T) == typeof(LocationSO))
            {
                value = (T)(object)GoapNodeOptionReader.GetOption<LocationSO>(locationNode, Value_Location.OptionValue, null);
                return true;
            }

            return false;
        }

        static bool TryResolveValueType(System.Type type, out GoapValueType valueType)
        {
            if (type == typeof(float))
            {
                valueType = GoapValueType.Float;
                return true;
            }
            if (type == typeof(bool))
            {
                valueType = GoapValueType.Bool;
                return true;
            }
            if (type == typeof(LocationSO))
            {
                valueType = GoapValueType.Location;
                return true;
            }

            valueType = default;
            return false;
        }

        static GoapVariableScope ResolveScope(VariableKind kind)
        {
            return kind == VariableKind.Local ? GoapVariableScope.Memory : GoapVariableScope.World;
        }

        static GoapValue ReadDefaultValue(IVariable variable, GoapValueType valueType)
        {
            switch (valueType)
            {
                case GoapValueType.Float:
                    if (variable.TryGetDefaultValue<float>(out var f))
                        return GoapValue.FromFloat(f);
                    break;
                case GoapValueType.Bool:
                    if (variable.TryGetDefaultValue<bool>(out var b))
                        return GoapValue.FromBool(b);
                    break;
                case GoapValueType.Location:
                    if (variable.TryGetDefaultValue<LocationSO>(out var loc))
                        return GoapValue.FromLocation(loc);
                    break;
            }

            return default;
        }
    }
}
