#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Heroes.Goap.Runtime.Agents;
using Heroes.Goap.Runtime.Planner;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.Goap.Editor
{
    public class GoapVisualDebuggerWindow : EditorWindow
    {
        const double RepaintHz = 5.0;

        GoapAgent _agent;
        GoapPlan _plan;
        bool _autoRefresh = true;
        double _lastRepaint;

        GoapDebugGraphView _graphView;
        IMGUIContainer _detailsPanel;

        [MenuItem("Tools/GOAP/Visual Debugger")]
        public static void Open()
        {
            var window = GetWindow<GoapVisualDebuggerWindow>();
            window.titleContent = new GUIContent("GOAP Visual Debugger");
            window.Show();
        }

        void OnEnable()
        {
            Selection.selectionChanged += HandleSelectionChanged;
            EditorApplication.update += HandleEditorUpdate;

            CreateUI();
            HandleSelectionChanged();
        }

        void OnDisable()
        {
            Selection.selectionChanged -= HandleSelectionChanged;
            EditorApplication.update -= HandleEditorUpdate;
        }

        void HandleSelectionChanged()
        {
            if (Selection.activeGameObject != null)
                _agent = Selection.activeGameObject.GetComponentInParent<GoapAgent>();

            Repaint();
        }

        void HandleEditorUpdate()
        {
            if (!_autoRefresh)
                return;

            var now = EditorApplication.timeSinceStartup;
            if (now - _lastRepaint < 1.0 / RepaintHz)
                return;

            _lastRepaint = now;
            Repaint();
        }

        void CreateUI()
        {
            rootVisualElement.Clear();

            var toolbar = new Toolbar();
            toolbar.Add(new ToolbarToggle { text = "Auto", value = _autoRefresh }.WithCallback(v => _autoRefresh = v));
            toolbar.Add(new ToolbarButton(() =>
            {
                if (_agent != null)
                    EditorGUIUtility.PingObject(_agent.gameObject);
            }) { text = "Ping" });
            rootVisualElement.Add(toolbar);

            var split = new TwoPaneSplitView(0, 600, TwoPaneSplitViewOrientation.Horizontal);
            _graphView = new GoapDebugGraphView();
            split.Add(_graphView);

            _detailsPanel = new IMGUIContainer(DrawDetailsPanel);
            var detailsScroll = new ScrollView();
            detailsScroll.Add(_detailsPanel);
            split.Add(detailsScroll);

            rootVisualElement.Add(split);
        }

        void OnGUI()
        {
            if (_agent == null)
                return;

            if (_autoRefresh || _plan == null)
                _plan = _agent.Plan();

            _graphView.Build(_agent, _plan);
        }

        void DrawDetailsPanel()
        {
            _agent = (GoapAgent)EditorGUILayout.ObjectField("Agent", _agent, typeof(GoapAgent), true);
            if (_agent == null)
            {
                EditorGUILayout.HelpBox("Select a GoapAgent in the scene.", MessageType.Info);
                return;
            }

            if (GUILayout.Button("Rebuild Plan", GUILayout.Width(120)))
                _plan = _agent.Plan();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current Goal", _agent.CurrentGoal != null ? _agent.CurrentGoal.Name : "(none)");
            EditorGUILayout.LabelField("Current Action", _agent.CurrentAction != null ? _agent.CurrentAction.Name : "(none)");
            EditorGUILayout.LabelField("Current Strategy", _agent.CurrentStrategy != null ? _agent.CurrentStrategy.name : "(none)");
            EditorGUILayout.LabelField("Strategy Node", _agent.CurrentStrategyNode != null ? _agent.CurrentStrategyNode.GetType().Name : "(none)");
            EditorGUILayout.LabelField("Strategy Port", string.IsNullOrWhiteSpace(_agent.CurrentStrategyPort) ? "(none)" : _agent.CurrentStrategyPort);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Goal Selection", EditorStyles.boldLabel);
            var debug = _agent.LastGoalDebug;
            if (debug != null)
            {
                foreach (var info in debug.OrderByDescending(d => d.Goal != null ? d.Goal.Priority : 0f))
                {
                    var goal = info.Goal;
                    if (goal == null)
                        continue;

                    var isChosen = _agent.CurrentGoal == goal;
                    var costText = info.Plan != null ? info.Plan.TotalCost.ToString("0.###") : "(no plan)";
                    if (info.Plan != null && info.Plan.Actions.Count == 0)
                        costText = "0 (already satisfied)";
                    var label = $"{goal.Name}  | Priority: {goal.Priority:0.###} | Cost: {costText}";
                    EditorGUILayout.LabelField(isChosen ? $"CHOSEN: {label}" : label);
                }
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Plan", EditorStyles.boldLabel);
            if (_agent.CurrentPlan == null || _agent.CurrentPlan.Actions == null)
            {
                EditorGUILayout.LabelField("(no plan)");
            }
            else
            {
                EditorGUILayout.LabelField("Total Cost", _agent.CurrentPlan.TotalCost.ToString("0.###"));
                if (_agent.CurrentPlan.Actions.Count == 0)
                {
                    EditorGUILayout.LabelField("(empty plan - goal already satisfied)");
                }
                else
                {
                    for (int i = 0; i < _agent.CurrentPlan.Actions.Count; i++)
                    {
                        var action = _agent.CurrentPlan.Actions[i];
                        EditorGUILayout.LabelField($"#{i + 1}: {action.Name}");
                    }
                }
            }

            EditorGUILayout.Space(8);
            DrawActionApplicabilityPanel(_agent);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("All Actions", EditorStyles.boldLabel);
            var actions = GetAllActions(_agent);
            if (actions.Count == 0)
            {
                EditorGUILayout.LabelField("(none)");
            }
            else
            {
                foreach (var action in actions.OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
                    EditorGUILayout.LabelField(action.Name);
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("World State", EditorStyles.boldLabel);
            DrawValueMap(_agent.WorldState?.Values);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Memory State", EditorStyles.boldLabel);
            DrawValueMap(_agent.MemoryState?.Values);
        }

        class GoapDebugGraphView : GraphView
        {
            const float NodeW = 170f;
            const float NodeH = 60f;
            const float Gap = 40f;

            public GoapDebugGraphView()
            {
                this.AddManipulator(new ContentDragger());
                this.AddManipulator(new SelectionDragger());
                this.AddManipulator(new RectangleSelector());
                this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
                this.StretchToParentSize();
            }

            public void Build(GoapAgent agent, GoapPlan plan)
            {
                DeleteElements(graphElements.ToList());

                if (agent == null)
                    return;

                BuildPlanGraph(agent, plan);
                BuildStrategyGraph(agent);
            }

            void BuildPlanGraph(GoapAgent agent, GoapPlan plan)
            {
                var x = 50f;
                var y = 50f;

                var goalNode = CreateNode(agent.CurrentGoal != null ? $"Goal\n{agent.CurrentGoal.Name}" : "Goal\n(none)", x, y, agent.CurrentGoal != null);
                AddElement(goalNode);

                x += NodeW + Gap;

                if (plan == null || plan.Actions == null || plan.Actions.Count == 0)
                    return;

                Node previous = goalNode;
                for (int i = 0; i < plan.Actions.Count; i++)
                {
                    var action = plan.Actions[i];
                    var isCurrent = agent.CurrentAction == action;
                    var node = CreateNode($"Action\n{action.Name}", x, y, isCurrent);
                    AddElement(node);
                    AddEdge(previous, node);
                    previous = node;
                    x += NodeW + Gap;
                }
            }

            void BuildStrategyGraph(GoapAgent agent)
            {
                var y = 170f;
                var x = 50f;

                var strategyName = agent.CurrentStrategy != null ? agent.CurrentStrategy.name : "(none)";
                var strategyNode = CreateNode($"Strategy\n{strategyName}", x, y, agent.CurrentStrategy != null);
                AddElement(strategyNode);

                x += NodeW + Gap;
                var nodeName = agent.CurrentStrategyNode != null ? agent.CurrentStrategyNode.GetType().Name : "(none)";
                var portName = string.IsNullOrWhiteSpace(agent.CurrentStrategyPort) ? string.Empty : $"\nPort: {agent.CurrentStrategyPort}";
                var currentNode = CreateNode($"Node\n{nodeName}{portName}", x, y, agent.CurrentStrategyNode != null);
                AddElement(currentNode);
                AddEdge(strategyNode, currentNode);
            }

            static Node CreateNode(string title, float x, float y, bool active)
            {
                var node = new Node { title = title };
                node.SetPosition(new Rect(x, y, NodeW, NodeH));
                var color = active ? new Color(0.25f, 0.6f, 0.9f) : new Color(0.2f, 0.2f, 0.2f);
                node.titleContainer.style.backgroundColor = color;
                return node;
            }

            void AddEdge(Node from, Node to)
            {
                var output = from.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                var input = to.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                from.outputContainer.Add(output);
                to.inputContainer.Add(input);
                from.RefreshPorts();
                to.RefreshPorts();

                var edge = new Edge { output = output, input = input };
                output.Connect(edge);
                input.Connect(edge);
                AddElement(edge);
            }
        }

        static List<Heroes.Goap.Runtime.Core.GoapActionDefinition> GetAllActions(GoapAgent agent)
        {
            var results = new Dictionary<string, Heroes.Goap.Runtime.Core.GoapActionDefinition>(StringComparer.OrdinalIgnoreCase);
            var archetype = agent.Archetype;
            while (archetype != null)
            {
                if (archetype.Graph != null)
                {
                    foreach (var action in archetype.Graph.Actions)
                        results[action.Id] = action;
                }
                archetype = archetype.Parent;
            }

            return results.Values.ToList();
        }

        static void DrawValueMap(IReadOnlyDictionary<string, Heroes.Goap.Runtime.Values.GoapValue> values)
        {
            if (values == null || values.Count == 0)
            {
                EditorGUILayout.LabelField("(empty)");
                return;
            }

            foreach (var pair in values.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
                EditorGUILayout.LabelField(pair.Key, pair.Value.ToString());
        }

        static void DrawActionApplicabilityPanel(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField("Action Applicability", EditorStyles.boldLabel);

                var state = BuildState(agent);
                var actions = GetAllActions(agent);
                if (actions.Count == 0)
                {
                    EditorGUILayout.LabelField("(no actions registered)");
                    return;
                }

                var applicableCount = 0;
                foreach (var action in actions.OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var applicable = IsActionApplicable(action, state);
                    if (applicable)
                        applicableCount++;

                    EditorGUILayout.LabelField($"{action.Name}  | {(applicable ? "APPLICABLE" : "BLOCKED")}");
                }

                EditorGUILayout.LabelField($"Applicable: {applicableCount} / {actions.Count}");

                if (agent.CurrentGoal != null)
                {
                    var goalSatisfied = IsGoalSatisfied(agent.CurrentGoal, state);
                    EditorGUILayout.LabelField($"Current Goal Satisfied: {(goalSatisfied ? "YES" : "NO")}");
                }
            }
        }

        static GoapState BuildState(GoapAgent agent)
        {
            var state = new GoapState();
            if (agent.WorldState != null)
            {
                foreach (var pair in agent.WorldState.Values)
                    state.Set(pair.Key, pair.Value);
            }

            if (agent.MemoryState != null)
            {
                foreach (var pair in agent.MemoryState.Values)
                    state.Set(pair.Key, pair.Value);
            }

            return state;
        }

        static bool IsActionApplicable(Heroes.Goap.Runtime.Core.GoapActionDefinition action, GoapState state)
        {
            if (action == null || action.Preconditions == null || action.Preconditions.Count == 0)
                return true;

            foreach (var condition in action.Preconditions)
            {
                if (condition == null || !condition.Evaluate(state))
                    return false;
            }

            return true;
        }

        static bool IsGoalSatisfied(Heroes.Goap.Runtime.Core.GoapGoalDefinition goal, GoapState state)
        {
            if (goal == null || goal.Desired == null || goal.Desired.Count == 0)
                return true;

            foreach (var condition in goal.Desired)
            {
                if (condition == null || !condition.Evaluate(state))
                    return false;
            }

            return true;
        }
    }

    static class ToolbarExtensions
    {
        public static ToolbarToggle WithCallback(this ToolbarToggle toggle, Action<bool> onChanged)
        {
            toggle.RegisterValueChangedCallback(evt => onChanged?.Invoke(evt.newValue));
            return toggle;
        }
    }
}
#endif
