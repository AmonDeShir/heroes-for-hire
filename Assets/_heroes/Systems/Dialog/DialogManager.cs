using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    public RuntimeDialogueGraph m_runtimeDialogueGraph;

    [SerializeField]
    private GameObject m_dialoguePanel;
    
    [SerializeField]
    private TextMeshProUGUI m_speakerText;

    [SerializeField]
    private TextMeshProUGUI m_dialogueText;
    
    [SerializeField]
    private Button m_dialogueButtonPrefab;
    
    [SerializeField]
    private Transform m_choicePanel;

    private Dictionary<string, RuntimeDialogueNode> _nodeLookup = new Dictionary<string, RuntimeDialogueNode>();
    private RuntimeDialogueNode _currentNode;

    private void Start()
    {
        foreach (var node in m_runtimeDialogueGraph.Nodes)
        {
            _nodeLookup[node.NodeID] = node;
        }

        if (!string.IsNullOrEmpty(m_runtimeDialogueGraph.EntryNodeID))
        {
            ShowNode(m_runtimeDialogueGraph.EntryNodeID);
        }
        else
        {
            EndDialogue();
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _currentNode != null && _currentNode.Choices.Count == 0)
        {
            if (!string.IsNullOrEmpty(_currentNode.NextNodeID))
            {
                ShowNode(_currentNode.NextNodeID);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void ShowNode(string nodeID)
    {
        if (!_nodeLookup.ContainsKey(nodeID))
        {
            Debug.LogError($"Node with ID {nodeID} not found in node lookup");
            EndDialogue();
            return;
        }
        
        if (!m_dialoguePanel.activeSelf)
        {
            m_dialoguePanel.SetActive(true);
        }
        
        _currentNode = _nodeLookup[nodeID];
        
        m_speakerText.text = _currentNode.SpeakerName;
        m_dialogueText.text = _currentNode.DialogueText;

        foreach (Transform child in m_choicePanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in _currentNode.Choices)
        {
            var button = Instantiate(m_dialogueButtonPrefab, m_choicePanel);
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = choice.ChoiceText;
            button.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(choice.DesinationNodeID))
                {
                    ShowNode(choice.DesinationNodeID);
                }
                else
                {
                    Debug.LogError($"Output node for choice not found in node lookup");
                    EndDialogue();
                }
            });
        }
    }

    private void EndDialogue()
    {
        m_dialoguePanel.SetActive(false);
        _currentNode = null;
        
        foreach (Transform child in m_choicePanel)
        {
            Destroy(child.gameObject);
        }
    }
}