using System.Collections.Generic;
using System.Linq;
using Heroes.Goap.Editor.Graphs;
using Heroes.Goap.Editor.Nodes;
using Heroes.Goap.Editor.Utilities;
using Heroes.Goap.Runtime.Strategies;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Heroes.Goap.Editor.Importers
{
    [ScriptedImporter(1, GoapStrategyGraph.AssetExtension)]
    internal class GoapStrategyGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<GoapStrategyGraph>(ctx.assetPath);
            if (graph == null)
            {
                Debug.LogError($"Failed to load GOAP strategy graph asset: {ctx.assetPath}");
                return;
            }

            var runtime = ScriptableObject.CreateInstance<GoapStrategyGraphAsset>();

            var nodes = graph.GetNodes().ToList();
            var nodeIds = new Dictionary<INode, int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeIds[nodes[i]] = i;
            }

            foreach (var node in nodes)
            {
                if (node is Strategy_Start)
                {
                    runtime.EntryNodeId = nodeIds[node];
                    runtime.Nodes.Add(new GoapStartNode { Id = nodeIds[node] });
                }
                else if (node is Strategy_Wait waitNode)
                {
                    var duration = ReadPortValue(waitNode.GetInputPortByName(Strategy_Wait.DurationPortName), GoapValueType.Float, 1f);
                    runtime.Nodes.Add(new GoapWaitNode
                    {
                        Id = nodeIds[node],
                        Duration = duration.FloatValue
                    });
                }
                else if (node is Strategy_SetValue_Float setNode)
                {
                    var variableRef = ReadVariableRef(setNode.GetInputPortByName(Strategy_SetValue_Float.VariablePortName));
                    var value = ReadPortValue(setNode.GetInputPortByName(Strategy_SetValue_Float.ValuePortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[node],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (node is Strategy_SetValue_Bool setBool)
                {
                    var variableRef = ReadVariableRef(setBool.GetInputPortByName(Strategy_SetValue_Bool.VariablePortName));
                    var value = ReadPortValue(setBool.GetInputPortByName(Strategy_SetValue_Bool.ValuePortName), GoapValueType.Bool, false);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[node],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (node is Strategy_SetValue_Location setLocation)
                {
                    var variableRef = ReadVariableRef(setLocation.GetInputPortByName(Strategy_SetValue_Location.VariablePortName));
                    var value = ReadPortValue(setLocation.GetInputPortByName(Strategy_SetValue_Location.ValuePortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapSetValueNode
                    {
                        Id = nodeIds[node],
                        VariableName = variableRef.Name,
                        Value = value
                    });
                }
                else if (node is Strategy_AddValue addNode)
                {
                    var variableRef = ReadVariableRef(addNode.GetInputPortByName(Strategy_AddValue.VariablePortName));
                    var delta = ReadPortValue(addNode.GetInputPortByName(Strategy_AddValue.DeltaPortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapAddValueNode
                    {
                        Id = nodeIds[node],
                        VariableName = variableRef.Name,
                        Delta = delta.FloatValue
                    });
                }
                else if (node is Strategy_If_Float ifFloat)
                {
                    var left = ReadPortValue(ifFloat.GetInputPortByName(Strategy_If_Float.ValueAPortName), GoapValueType.Float, 0f);
                    var right = ReadPortValue(ifFloat.GetInputPortByName(Strategy_If_Float.ValueBPortName), GoapValueType.Float, 0f);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[node],
                        Operator = GoapNodeOptionReader.GetOption(ifFloat, Strategy_If_Float.OptionOperator, GoapConditionOp.GreaterOrEqual),
                        Left = left,
                        Right = right
                    });
                }
                else if (node is Strategy_If_Bool ifBool)
                {
                    var left = ReadPortValue(ifBool.GetInputPortByName(Strategy_If_Bool.ValueAPortName), GoapValueType.Bool, false);
                    var right = ReadPortValue(ifBool.GetInputPortByName(Strategy_If_Bool.ValueBPortName), GoapValueType.Bool, false);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[node],
                        Operator = GoapNodeOptionReader.GetOption(ifBool, Strategy_If_Bool.OptionOperator, GoapConditionOp.Equal),
                        Left = left,
                        Right = right
                    });
                }
                else if (node is Strategy_If_Location ifLocation)
                {
                    var left = ReadPortValue(ifLocation.GetInputPortByName(Strategy_If_Location.ValueAPortName), GoapValueType.Location, null);
                    var right = ReadPortValue(ifLocation.GetInputPortByName(Strategy_If_Location.ValueBPortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapIfNode
                    {
                        Id = nodeIds[node],
                        Operator = GoapNodeOptionReader.GetOption(ifLocation, Strategy_If_Location.OptionOperator, GoapConditionOp.Equal),
                        Left = left,
                        Right = right
                    });
                }
                else if (node is Strategy_Random randomNode)
                {
                    var chance = ReadPortValue(randomNode.GetInputPortByName(Strategy_Random.ChancePortName), GoapValueType.Float, 0.5f);
                    runtime.Nodes.Add(new GoapRandomNode
                    {
                        Id = nodeIds[node],
                        ChanceA = chance.FloatValue
                    });
                }
                else if (node is Strategy_PlayAnimation animNode)
                {
                    var animation = ReadStringPortValue(animNode.GetInputPortByName(Strategy_PlayAnimation.AnimationPortName), string.Empty);
                    runtime.Nodes.Add(new GoapPlayAnimationNode
                    {
                        Id = nodeIds[node],
                        AnimationName = animation
                    });
                }
                else if (node is Strategy_MoveTo moveNode)
                {
                    var locationValue = ReadPortValue(moveNode.GetInputPortByName(Strategy_MoveTo.ValuePortName), GoapValueType.Location, null);
                    runtime.Nodes.Add(new GoapMoveToLocationNode
                    {
                        Id = nodeIds[node],
                        Target = locationValue.LocationValue
                    });
                }
                else if (node is Strategy_Wander wanderNode)
                {
                    var radius = ReadPortValue(wanderNode.GetInputPortByName(Strategy_Wander.RadiusPortName), GoapValueType.Float, 5f);
                    runtime.Nodes.Add(new GoapWanderNode
                    {
                        Id = nodeIds[node],
                        Radius = radius.FloatValue
                    });
                }
            }

            foreach (var node in nodes)
            {
                var outputs = node.GetOutputPorts();
                foreach (var output in outputs)
                {
                    var connected = new List<IPort>();
                    output.GetConnectedPorts(connected);
                    foreach (var connectedPort in connected)
                    {
                        var toNode = connectedPort.GetNode();
                        if (toNode == null)
                            continue;

                        runtime.Edges.Add(new GoapStrategyEdge
                        {
                            FromNodeId = nodeIds[node],
                            FromPortName = output.name,
                            ToNodeId = nodeIds[toNode]
                        });
                    }
                }
            }

            ctx.AddObjectToAsset("Runtime", runtime);
            ctx.SetMainObject(runtime);
        }

        static GoapValue ReadPortValue(IPort port, GoapValueType valueType, object fallback)
        {
            if (TryReadConnectedValue(port, valueType, out var connectedValue))
                return connectedValue;

            return valueType switch
            {
                GoapValueType.Float => GoapValue.FromFloat(fallback is float f ? f : 0f),
                GoapValueType.Bool => GoapValue.FromBool(fallback is bool b && b),
                GoapValueType.Location => GoapValue.FromLocation(fallback as LocationSO),
                _ => default
            };
        }

        static bool TryReadConnectedValue(IPort port, GoapValueType valueType, out GoapValue value)
        {
            value = default;
            if (port == null)
                return false;

            var connected = new List<IPort>();
            port.GetConnectedPorts(connected);
            if (connected.Count == 0)
                return false;

            var node = connected[0].GetNode();
            switch (valueType)
            {
                case GoapValueType.Float:
                    if (node is Value_Float floatNode)
                    {
                        value = GoapValue.FromFloat(GoapNodeOptionReader.GetOption(floatNode, Value_Float.OptionValue, 0f));
                        return true;
                    }
                    break;
                case GoapValueType.Bool:
                    if (node is Value_Bool boolNode)
                    {
                        value = GoapValue.FromBool(GoapNodeOptionReader.GetOption(boolNode, Value_Bool.OptionValue, false));
                        return true;
                    }
                    break;
                case GoapValueType.Location:
                    if (node is Value_Location locationNode)
                    {
                        value = GoapValue.FromLocation(GoapNodeOptionReader.GetOption<LocationSO>(locationNode, Value_Location.OptionValue, null));
                        return true;
                    }
                    break;
            }

            return false;
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

        static string ReadStringPortValue(IPort port, string fallback)
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
    }
}
