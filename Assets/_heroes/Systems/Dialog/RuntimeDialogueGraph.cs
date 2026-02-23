using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDialogueGraph : ScriptableObject
{
    public string EntryNodeID;
    public List<RuntimeDialogueNode> Nodes = new List<RuntimeDialogueNode>();
}

[Serializable]
public class RuntimeDialogueNode
{
    public string NodeID;
    public string NextNodeID;
    public string SpeakerName;
    public string DialogueText;
    public List<ChoiceData> Choices = new List<ChoiceData>();
}

[Serializable]
public class ChoiceData
{
    public string ChoiceText;
    public string DesinationNodeID;
}