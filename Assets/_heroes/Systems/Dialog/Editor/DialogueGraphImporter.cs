using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEditor.AssetImporters;


[ScriptedImporter(1, DialogueGraph.AssetExtension)]
public class DialogueGraphImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var editor = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
        var runtime = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();
        var nodeIDMap = new Dictionary<INode, string>();

        foreach (var node in editor.GetNodes())
        {
            nodeIDMap[node] = Guid.NewGuid().ToString();
        }

        var startNode = editor.GetNodes().OfType<StartNode>().FirstOrDefault();

        if (startNode != null)
        {
            var entryPoint = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;

            if (entryPoint != null)
            {
                runtime.EntryNodeID = nodeIDMap[entryPoint.GetNode()];
            }
        }

        foreach (var iNode in editor.GetNodes())
        {
            if (iNode is StartNode || iNode is EndNode)
            {
                continue;
            }

            var runtimeNode = new RuntimeDialogueNode { NodeID = nodeIDMap[iNode] };

            if (iNode is DialogueNode dialogueNode)
            {
                ProcessDialogueNode(dialogueNode, runtimeNode, nodeIDMap);
            }
            else if (iNode is ChoiceNode choiceNode)
            {
                ProcessChoiceDialogueNode(choiceNode, runtimeNode, nodeIDMap);
            }
            
            runtime.Nodes.Add(runtimeNode);
        }
        
        ctx.AddObjectToAsset("RuntimeData", runtime);
        ctx.SetMainObject(runtime);
    }

    private void ProcessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeMaps)
    {
        runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName("Speaker"));
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue"));

        var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort;

        if (nextNodePort != null)
        {
            var nextIsSupported =  nextNodePort.GetNode() is DialogueNode || nextNodePort.GetNode() is ChoiceNode;
            runtimeNode.NextNodeID = nextIsSupported ? nodeMaps[nextNodePort.GetNode()] : "";
        }
    }

    private void ProcessChoiceDialogueNode(ChoiceNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeMaps)
    {
        runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName("Speaker"));
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue"));

        var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith("Choice "));

        foreach (var port in choiceOutputPorts)
        {
            var index = port.name.Substring("Choice ".Length);
            var textPort = node.GetInputPortByName($"Choice Text {index}");

            var data = new ChoiceData()
            {
                ChoiceText = GetPortValue<string>(textPort),
                DesinationNodeID = port.firstConnectedPort.GetNode() != null
                    ? nodeMaps[port.firstConnectedPort.GetNode()]
                    : ""
            };
            
            runtimeNode.Choices.Add(data);
        }
    }

    
    private T GetPortValue<T>(IPort port)
    {
        if (port == null)
        {
            return default;
        }

        if (port.isConnected)
        {
            if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
            {
                variableNode.variable.TryGetDefaultValue(out T value);
                return value;
            }
        }
        
        port.TryGetValue(out T fallbackValue);
        return fallbackValue;
    }
}
