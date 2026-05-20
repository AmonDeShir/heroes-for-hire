using System;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Heroes.GOAP.Core.Debug;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes.GOAP.Editor
{
    public sealed class GoapDebuggerWindow : EditorWindow
    {
        private const string UxmlPath = "Assets/_heroes/Frameworks/GOAP Lib/Editor/GoapDebuggerWindow.uxml";
        private const string UssPath = "Assets/_heroes/Frameworks/GOAP Lib/Editor/GoapDebuggerWindow.uss";
        private const float RefreshIntervalSeconds = 0.2f;

        private PopupField<AgentOption> agentDropdown;
        private Toggle autoRefreshToggle;
        private Button refreshButton;
        private Button useSelectionButton;

        private Button tabPlanButton;
        private Button tabGoalsButton;
        private Button tabActionsButton;
        private Button tabMemoryButton;
        private Button tabWorldButton;

        private VisualElement planTab;
        private VisualElement goalsTab;
        private VisualElement actionsTab;
        private VisualElement memoryTab;
        private VisualElement worldTab;

        private Label planGoalLabel;
        private Label planIdleLabel;
        private ListView planStepsList;
        private Label stepNameLabel;
        private Label stepDescriptionLabel;
        private Label stepTimeLabel;
        private Label stepPreconditionsLabel;
        private Label stepEffectLabel;
        private Label previewGoalLabel;
        private Label previewLocationLabel;
        private MultiColumnListView previewMemoryList;
        private VisualElement previewPanel;
        private VisualElement previewHandle;
        private PlanGraphElement planGraph;
        private ScrollView planGraphScroll;

        private MultiColumnListView goalsList;
        private MultiColumnListView actionsList;
        private MultiColumnListView memoryList;
        private Label memoryLocationLabel;
        private Label worldVersionLabel;
        private Label worldValidLabel;
        private VisualElement worldDetailsRoot;

        private readonly List<AgentOption> agentOptions = new List<AgentOption>();
        private GoapDebugSnapshot currentSnapshot;
        private IVisualElementScheduledItem refreshSchedule;
        private bool forceRefresh;
        private string lastPlanSignature;
        private int previewStepIndex = -1;
        private bool isDraggingPreview;
        private Vector2 previewDragStart;
        private Vector2 previewPanelStart;

        [MenuItem("Tools/GOAP/Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow<GoapDebuggerWindow>();
            window.titleContent = new GUIContent("GOAP Debugger");
            window.minSize = new Vector2(720f, 420f);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            var style = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            if (style != null)
            {
                rootVisualElement.styleSheets.Add(style);
            }

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            if (visualTree == null)
            {
                rootVisualElement.Add(new Label("Missing UXML: " + UxmlPath));
                return;
            }

            var root = visualTree.Instantiate();
            rootVisualElement.Add(root);

            BindToolbar(root);
            BindTabs(root);
            BindPlanTab(root);
            BindGoalsTab(root);
            BindActionsTab(root);
            BindMemoryTab(root);
            BindWorldTab(root);

            ShowTab(planTab, tabPlanButton);

            refreshSchedule = rootVisualElement.schedule.Execute(RefreshSnapshot).Every((long)(RefreshIntervalSeconds * 1000f));
            RefreshAgentList();
            UseSelection();
            RefreshSnapshot();
        }

        private void BindToolbar(VisualElement root)
        {
            useSelectionButton = root.Q<Button>("use-selection-button");
            autoRefreshToggle = root.Q<Toggle>("auto-toggle");
            refreshButton = root.Q<Button>("refresh-button");

            useSelectionButton.clicked += UseSelection;
            refreshButton.clicked += ForceRefresh;

            autoRefreshToggle.value = true;

            var dropdownPlaceholder = root.Q<VisualElement>("agent-dropdown");
            agentDropdown = new PopupField<AgentOption>(agentOptions, 0, FormatAgent, FormatAgent);
            agentDropdown.name = "agent-dropdown";
            agentDropdown.AddToClassList("goap-debugger__agent-dropdown");
            agentDropdown.RegisterValueChangedCallback(OnAgentSelected);
            ReplaceElement(dropdownPlaceholder, agentDropdown);
        }

        private void BindTabs(VisualElement root)
        {
            tabPlanButton = root.Q<Button>("tab-plan");
            tabGoalsButton = root.Q<Button>("tab-goals");
            tabActionsButton = root.Q<Button>("tab-actions");
            tabMemoryButton = root.Q<Button>("tab-memory");
            tabWorldButton = root.Q<Button>("tab-world");

            planTab = root.Q<VisualElement>("plan-tab");
            goalsTab = root.Q<VisualElement>("goals-tab");
            actionsTab = root.Q<VisualElement>("actions-tab");
            memoryTab = root.Q<VisualElement>("memory-tab");
            worldTab = root.Q<VisualElement>("world-tab");

            tabPlanButton.clicked += () => ShowTab(planTab, tabPlanButton);
            tabGoalsButton.clicked += () => ShowTab(goalsTab, tabGoalsButton);
            tabActionsButton.clicked += () => ShowTab(actionsTab, tabActionsButton);
            tabMemoryButton.clicked += () => ShowTab(memoryTab, tabMemoryButton);
            tabWorldButton.clicked += () => ShowTab(worldTab, tabWorldButton);
        }

        private void BindPlanTab(VisualElement root)
        {
            planGoalLabel = root.Q<Label>("plan-goal");
            planIdleLabel = root.Q<Label>("plan-idle");
            planStepsList = root.Q<ListView>("plan-steps-list");
            stepNameLabel = root.Q<Label>("step-name");
            stepDescriptionLabel = root.Q<Label>("step-description");
            stepTimeLabel = root.Q<Label>("step-time");
            stepPreconditionsLabel = root.Q<Label>("step-preconditions");
            stepEffectLabel = root.Q<Label>("step-effect");
            previewGoalLabel = root.Q<Label>("preview-goal");
            previewLocationLabel = root.Q<Label>("preview-location");
            previewMemoryList = root.Q<MultiColumnListView>("preview-memory-list");
            previewPanel = root.Q<VisualElement>("preview-panel");
            previewHandle = root.Q<VisualElement>("preview-handle");
            planGraphScroll = root.Q<ScrollView>("plan-graph-scroll");

            var graphPlaceholder = root.Q<VisualElement>("plan-graph");
            planGraph = new PlanGraphElement();
            planGraph.name = "plan-graph";
            planGraph.AddToClassList("goap-debugger__plan-graph");
            ReplaceElement(graphPlaceholder, planGraph);

            planGraphScroll.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            planGraphScroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            RegisterPreviewDragHandlers();

            previewMemoryList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            previewMemoryList.fixedItemHeight = 22f;
            previewMemoryList.selectionType = SelectionType.None;
            var previewColumns = previewMemoryList.columns;
            previewColumns.Clear();
            previewColumns.Add(MakePreviewMemoryColumn("Name", 160f, belief => string.IsNullOrEmpty(belief.Name) ? $"Belief[{belief.Index}]" : belief.Name, true));
            previewColumns.Add(MakePreviewMemoryColumn("Index", 80f, belief => belief.Index.ToString(), false));
            previewColumns.Add(MakePreviewMemoryColumn("Value", 100f, belief => belief.Value.ToString("0.###"), false));

            planStepsList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            planStepsList.fixedItemHeight = 22f;
            planStepsList.selectionType = SelectionType.Single;
            planStepsList.makeItem = () =>
            {
                var label = new Label();
                label.AddToClassList("goap-debugger__plan-step");
                return label;
            };
            planStepsList.bindItem = (element, i) =>
            {
                var label = (Label)element;
                var step = currentSnapshot?.Plan?.Steps != null && i < currentSnapshot.Plan.Steps.Count
                    ? currentSnapshot.Plan.Steps[i]
                    : null;
                label.text = step != null ? step.Name : "-";
            };
            planStepsList.onSelectionChange += _ => UpdateStepDetails();
        }

        private void RegisterPreviewDragHandlers()
        {
            if (previewPanel == null || previewHandle == null)
            {
                return;
            }

            previewHandle.RegisterCallback<PointerDownEvent>(evt =>
            {
                isDraggingPreview = true;
                previewDragStart = evt.position;
                previewPanelStart = new Vector2(previewPanel.resolvedStyle.left, previewPanel.resolvedStyle.top);
                previewHandle.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            });

            previewHandle.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!isDraggingPreview)
                {
                    return;
                }

                var delta = new Vector2(evt.position.x, evt.position.y) - previewDragStart;
                var newLeft = previewPanelStart.x + delta.x;
                var newTop = previewPanelStart.y + delta.y;
                previewPanel.style.left = newLeft;
                previewPanel.style.top = newTop;
                evt.StopPropagation();
            });

            previewHandle.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (!isDraggingPreview)
                {
                    return;
                }

                isDraggingPreview = false;
                previewHandle.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
            });
        }

        private void BindGoalsTab(VisualElement root)
        {
            goalsList = root.Q<MultiColumnListView>("goals-list");
            goalsList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            goalsList.fixedItemHeight = 22f;
            goalsList.selectionType = SelectionType.None;

            var columns = goalsList.columns;
            columns.Clear();
            columns.Add(MakeGoalColumn("Name", 220f, goal => goal.Name, true));
            columns.Add(MakeGoalColumn("Priority", 80f, goal => goal.Priority.ToString(), false));
            columns.Add(MakeGoalColumn("Importance", 110f, goal => goal.Importance.ToString("0.###"), false));
            columns.Add(MakeGoalColumn("Heuristic", 110f, goal => goal.Heuristic.ToString("0.###"), false));
            columns.Add(MakeGoalColumn("Achieved", 90f, goal => goal.Achieved.ToString(), false));
        }

        private void BindActionsTab(VisualElement root)
        {
            actionsList = root.Q<MultiColumnListView>("actions-list");
            actionsList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            actionsList.fixedItemHeight = 22f;
            actionsList.selectionType = SelectionType.None;

            var columns = actionsList.columns;
            columns.Clear();
            columns.Add(MakeActionColumn("Name", 200f, action => action.Name, true));
            columns.Add(MakeActionColumn("Preconditions", 240f, action => action.PreconditionsLabel, true));
            columns.Add(MakeActionColumn("Effect", 240f, action => action.EffectLabel, true));
            columns.Add(MakeActionColumn("Time", 80f, action => action.EstimatedTime.ToString("0.###"), false));
        }

        private Column MakeActionColumn(string title, float width, Func<GoapDebugAction, string> valueSelector, bool stretch)
        {
            return new Column
            {
                title = title,
                width = width,
                stretchable = stretch,
                makeCell = () => new Label(),
                bindCell = (element, i) => BindActionCell((Label)element, i, valueSelector)
            };
        }

        private void BindActionCell(Label label, int index, Func<GoapDebugAction, string> valueSelector)
        {
            if (currentSnapshot?.Actions == null || index >= currentSnapshot.Actions.Count)
            {
                label.text = string.Empty;
                return;
            }

            var action = currentSnapshot.Actions[index];
            label.text = valueSelector(action);
        }

        private Column MakeGoalColumn(string title, float width, Func<GoapDebugGoal, string> valueSelector, bool stretch)
        {
            return new Column
            {
                title = title,
                width = width,
                stretchable = stretch,
                makeCell = () => new Label(),
                bindCell = (element, i) => BindGoalCell((Label)element, i, valueSelector)
            };
        }

        private void BindGoalCell(Label label, int index, Func<GoapDebugGoal, string> valueSelector)
        {
            if (currentSnapshot?.Goals == null || index >= currentSnapshot.Goals.Count)
            {
                label.text = string.Empty;
                label.EnableInClassList("goap-debugger__goal-cell--current", false);
                return;
            }

            var goal = currentSnapshot.Goals[index];
            label.text = valueSelector(goal);
            var isCurrent = !string.IsNullOrEmpty(goal.Name) && currentSnapshot?.Plan?.GoalName == goal.Name;
            label.EnableInClassList("goap-debugger__goal-cell--current", isCurrent);
        }

        private void BindMemoryTab(VisualElement root)
        {
            memoryLocationLabel = root.Q<Label>("memory-location");
            memoryList = root.Q<MultiColumnListView>("memory-list");
            memoryList.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            memoryList.fixedItemHeight = 22f;
            memoryList.selectionType = SelectionType.None;

            var columns = memoryList.columns;
            columns.Clear();
            columns.Add(MakeMemoryColumn("Name", 160f, belief => string.IsNullOrEmpty(belief.Name) ? $"Belief[{belief.Index}]" : belief.Name, true));
            columns.Add(MakeMemoryColumn("Index", 80f, belief => belief.Index.ToString(), false));
            columns.Add(MakeMemoryColumn("Value", 120f, belief => belief.Value.ToString("0.###"), false));
        }

        private Column MakeMemoryColumn(string title, float width, Func<GoapDebugBelief, string> valueSelector, bool stretch)
        {
            return new Column
            {
                title = title,
                width = width,
                stretchable = stretch,
                makeCell = () => new Label(),
                bindCell = (element, i) => BindMemoryCell((Label)element, i, valueSelector)
            };
        }

        private void BindMemoryCell(Label label, int index, Func<GoapDebugBelief, string> valueSelector)
        {
            if (currentSnapshot?.Memory?.Beliefs == null || index >= currentSnapshot.Memory.Beliefs.Count)
            {
                label.text = string.Empty;
                return;
            }

            var belief = currentSnapshot.Memory.Beliefs[index];
            label.text = valueSelector(belief);
        }

        private Column MakePreviewMemoryColumn(string title, float width, Func<GoapDebugBelief, string> valueSelector, bool stretch)
        {
            return new Column
            {
                title = title,
                width = width,
                stretchable = stretch,
                makeCell = () => new Label(),
                bindCell = (element, i) => BindPreviewMemoryCell((Label)element, i, valueSelector)
            };
        }

        private void BindPreviewMemoryCell(Label label, int index, Func<GoapDebugBelief, string> valueSelector)
        {
            var plan = currentSnapshot?.Plan;
            var selectedIndex = previewStepIndex;
            if (plan == null || selectedIndex < 0 || selectedIndex >= plan.Steps.Count)
            {
                label.text = string.Empty;
                return;
            }

            var beliefs = plan.Steps[selectedIndex].PreviewBeliefs;
            if (beliefs == null || index >= beliefs.Count)
            {
                label.text = string.Empty;
                return;
            }

            var belief = beliefs[index];
            label.text = valueSelector(belief);
        }

        private void BindWorldTab(VisualElement root)
        {
            worldVersionLabel = root.Q<Label>("world-version");
            worldValidLabel = root.Q<Label>("world-valid");
            worldDetailsRoot = root.Q<VisualElement>("world-details");
        }

        private void ShowTab(VisualElement target, Button activeButton)
        {
            planTab.style.display = target == planTab ? DisplayStyle.Flex : DisplayStyle.None;
            goalsTab.style.display = target == goalsTab ? DisplayStyle.Flex : DisplayStyle.None;
            actionsTab.style.display = target == actionsTab ? DisplayStyle.Flex : DisplayStyle.None;
            memoryTab.style.display = target == memoryTab ? DisplayStyle.Flex : DisplayStyle.None;
            worldTab.style.display = target == worldTab ? DisplayStyle.Flex : DisplayStyle.None;

            SetTabButtonActive(tabPlanButton, activeButton == tabPlanButton);
            SetTabButtonActive(tabGoalsButton, activeButton == tabGoalsButton);
            SetTabButtonActive(tabActionsButton, activeButton == tabActionsButton);
            SetTabButtonActive(tabMemoryButton, activeButton == tabMemoryButton);
            SetTabButtonActive(tabWorldButton, activeButton == tabWorldButton);
        }

        private static void SetTabButtonActive(Button button, bool active)
        {
            if (button == null)
            {
                return;
            }

            button.EnableInClassList("goap-debugger__tab-button--active", active);
        }

        private void RefreshSnapshot()
        {
            if (!EditorApplication.isPlaying)
            {
                currentSnapshot = null;
                UpdateUIForEditMode();
                return;
            }

            if (autoRefreshToggle != null && !autoRefreshToggle.value && !forceRefresh)
            {
                return;
            }

            forceRefresh = false;

            var source = GetSelectedSource();
            if (source is UnityEngine.Object unityObject && unityObject == null)
            {
                RefreshAgentList();
                source = GetSelectedSource();
            }
            if (source == null || !source.TryGetSnapshot(out currentSnapshot))
            {
                currentSnapshot = null;
                UpdateUIEmpty();
                return;
            }

            UpdateUIFromSnapshot();
        }

        private void ForceRefresh()
        {
            forceRefresh = true;
            RefreshSnapshot();
        }

        private void UpdateUIForEditMode()
        {
            planGoalLabel.text = "Plan: (Play Mode only)";
            planIdleLabel.text = "Idle: -";
            planStepsList.itemsSource = Array.Empty<GoapDebugPlanStep>();
            planStepsList.RefreshItems();
            planGraph.SetSteps(Array.Empty<GoapDebugPlanStep>(), -1);

            stepNameLabel.text = "Step: -";
            stepDescriptionLabel.text = "Description: -";
            stepTimeLabel.text = "Time: -";
            stepPreconditionsLabel.text = "Preconditions: -";
            stepEffectLabel.text = "Effect: -";
            previewGoalLabel.text = "Goal: -";
            previewLocationLabel.text = "Preview Location: -";
            previewMemoryList.itemsSource = Array.Empty<GoapDebugBelief>();
            previewMemoryList.Rebuild();
            previewStepIndex = -1;

            goalsList.itemsSource = Array.Empty<GoapDebugGoal>();
            goalsList.Rebuild();

            actionsList.itemsSource = Array.Empty<GoapDebugAction>();
            actionsList.Rebuild();

            actionsList.itemsSource = Array.Empty<GoapDebugAction>();
            actionsList.Rebuild();

            memoryLocationLabel.text = "Location: -";
            memoryList.itemsSource = Array.Empty<GoapDebugBelief>();
            memoryList.Rebuild();

            worldVersionLabel.text = "Version: -";
            worldValidLabel.text = "IsValid: -";
            worldDetailsRoot.Clear();
            worldDetailsRoot.Add(new Label("Enter Play Mode to view GOAP debug data."));
        }

        private void UpdateUIEmpty()
        {
            planGoalLabel.text = "Plan: -";
            planIdleLabel.text = "Idle: -";
            planStepsList.itemsSource = Array.Empty<GoapDebugPlanStep>();
            planStepsList.RefreshItems();
            planGraph.SetSteps(Array.Empty<GoapDebugPlanStep>(), -1);

            stepNameLabel.text = "Step: -";
            stepDescriptionLabel.text = "Description: -";
            stepTimeLabel.text = "Time: -";
            stepPreconditionsLabel.text = "Preconditions: -";
            stepEffectLabel.text = "Effect: -";
            previewGoalLabel.text = "Goal: -";
            previewLocationLabel.text = "Preview Location: -";
            previewMemoryList.itemsSource = Array.Empty<GoapDebugBelief>();
            previewMemoryList.Rebuild();
            previewStepIndex = -1;
            previewGoalLabel.text = "Goal: -";
            previewLocationLabel.text = "Preview Location: -";
            previewMemoryList.itemsSource = Array.Empty<GoapDebugBelief>();
            previewMemoryList.Rebuild();

            goalsList.itemsSource = Array.Empty<GoapDebugGoal>();
            goalsList.Rebuild();

            memoryLocationLabel.text = "Location: -";
            memoryList.itemsSource = Array.Empty<GoapDebugBelief>();
            memoryList.Rebuild();

            worldVersionLabel.text = "Version: -";
            worldValidLabel.text = "IsValid: -";
            worldDetailsRoot.Clear();
            worldDetailsRoot.Add(new Label("No world snapshot."));
        }

        private void UpdateUIFromSnapshot()
        {
            var plan = currentSnapshot.Plan;
            var planSignature = BuildPlanSignature(plan);
            var planChanged = planSignature != lastPlanSignature;
            lastPlanSignature = planSignature;
            planGoalLabel.text = string.IsNullOrEmpty(plan.GoalName) ? "Plan: -" : $"Plan: {plan.GoalName}";
            if (currentSnapshot.Idle != null && currentSnapshot.Idle.IsActive)
            {
                var idleName = string.IsNullOrEmpty(currentSnapshot.Idle.Name) ? "Idle" : currentSnapshot.Idle.Name;
                planIdleLabel.text = $"Idle: {idleName} (active)";
            }
            else
            {
                if (currentSnapshot.Idle != null && !string.IsNullOrEmpty(currentSnapshot.Idle.Name))
                {
                    planIdleLabel.text = $"Idle: {currentSnapshot.Idle.Name} (inactive)";
                }
                else
                {
                    planIdleLabel.text = "Idle: -";
                }
            }

            planStepsList.itemsSource = plan.Steps as IList;
            planStepsList.RefreshItems();
            if (plan.Steps.Count == 0)
            {
                planStepsList.ClearSelection();
            }
            else if (planChanged || planStepsList.selectedIndex < 0 || planStepsList.selectedIndex >= plan.Steps.Count)
            {
                var targetIndex = plan.CurrentStepIndex >= 0 && plan.CurrentStepIndex < plan.Steps.Count
                    ? plan.CurrentStepIndex
                    : 0;
                planStepsList.SetSelection(targetIndex);
            }

            planGraph.SetSteps(plan.Steps, plan.CurrentStepIndex);
            UpdateStepDetails();

            goalsList.itemsSource = currentSnapshot.Goals as IList;
            goalsList.Rebuild();

            actionsList.itemsSource = currentSnapshot.Actions as IList;
            actionsList.Rebuild();

            memoryLocationLabel.text = $"Location: {currentSnapshot.Memory.Location}";
            memoryList.itemsSource = currentSnapshot.Memory.Beliefs as IList;
            memoryList.Rebuild();

            worldVersionLabel.text = $"Version: {currentSnapshot.World.Version}";
            worldValidLabel.text = $"IsValid: {currentSnapshot.World.IsValid}";
            UpdateWorldDetails(currentSnapshot.World.Snapshot);
        }

        private void UpdateStepDetails()
        {
            var plan = currentSnapshot?.Plan;
            if (plan == null || plan.Steps.Count == 0)
            {
                stepNameLabel.text = "Step: -";
                stepDescriptionLabel.text = "Description: -";
                stepTimeLabel.text = "Time: -";
                stepPreconditionsLabel.text = "Preconditions: -";
                stepEffectLabel.text = "Effect: -";
                previewGoalLabel.text = "Goal: -";
                previewLocationLabel.text = "Preview Location: -";
                previewMemoryList.itemsSource = Array.Empty<GoapDebugBelief>();
                previewMemoryList.Rebuild();
                previewStepIndex = -1;
                return;
            }

            var index = planStepsList.selectedIndex;
            if (index < 0 || index >= plan.Steps.Count)
            {
                index = plan.CurrentStepIndex >= 0 ? plan.CurrentStepIndex : 0;
            }

            previewStepIndex = index;

            var step = plan.Steps[index];
            stepNameLabel.text = string.IsNullOrEmpty(step.Name) ? "Step: -" : $"Step: {step.Name}";
            stepDescriptionLabel.text = string.IsNullOrEmpty(step.Description) ? "Description: -" : $"Description: {step.Description}";
            stepTimeLabel.text = $"Time: {step.EstimatedTime:0.###}";
            stepPreconditionsLabel.text = step.PreconditionsLabel;
            stepEffectLabel.text = step.EffectLabel;
            var goalName = currentSnapshot?.Plan?.GoalName;
            var goalTitle = string.IsNullOrEmpty(goalName) ? "Goal" : $"Goal: {goalName}";
            previewGoalLabel.text = $"{goalTitle} | Achieved={step.GoalAchieved}, Heuristic={step.GoalHeuristic:0.###}";
            previewLocationLabel.text = $"Preview Location: {step.PreviewLocation}";
            previewMemoryList.itemsSource = step.PreviewBeliefs as IList;
            previewMemoryList.Rebuild();
        }

        private static string BuildPlanSignature(GoapDebugPlan plan)
        {
            if (plan == null || plan.Steps == null || plan.Steps.Count == 0)
            {
                return string.Empty;
            }

            var names = new string[plan.Steps.Count];
            for (var i = 0; i < plan.Steps.Count; i++)
            {
                names[i] = plan.Steps[i].Name ?? string.Empty;
            }

            return string.Concat(plan.GoalName ?? string.Empty, "|", plan.Steps.Count, "|", string.Join(",", names));
        }

        private void UpdateWorldDetails(object snapshot)
        {
            var renderer = GoapWorldDebugRendererRegistry.Resolve(snapshot);
            renderer.Render(snapshot, worldDetailsRoot);
        }

        private void RefreshAgentList()
        {
            agentOptions.Clear();
            foreach (var source in FindAllSources())
            {
                agentOptions.Add(new AgentOption(source));
            }

            if (agentOptions.Count == 0)
            {
                agentOptions.Add(new AgentOption(null));
            }

            agentDropdown.choices = agentOptions;
            agentDropdown.SetValueWithoutNotify(agentOptions[0]);
        }

        private void OnAgentSelected(ChangeEvent<AgentOption> evt)
        {
            RefreshSnapshot();
        }

        private void UseSelection()
        {
            var source = FindSourceOnSelection();
            if (source == null)
            {
                return;
            }

            if (!agentOptions.Any(option => option.Source == source))
            {
                RefreshAgentList();
            }

            var selected = agentOptions.FirstOrDefault(option => option.Source == source);
            if (selected != null)
            {
                agentDropdown.SetValueWithoutNotify(selected);
            }

            ForceRefresh();
        }

        private void OnSelectionChanged()
        {
            if (agentDropdown == null)
            {
                return;
            }

            var source = FindSourceOnSelection();
            if (source == null)
            {
                return;
            }

            if (!agentOptions.Any(option => option.Source == source))
            {
                RefreshAgentList();
            }

            var selected = agentOptions.FirstOrDefault(option => option.Source == source);
            if (selected != null)
            {
                agentDropdown.SetValueWithoutNotify(selected);
                ForceRefresh();
            }
        }

        private IGoapDebugSource GetSelectedSource()
        {
            return agentDropdown?.value?.Source;
        }

        private static string FormatAgent(AgentOption option)
        {
            return option != null ? option.Label : "<none>";
        }

        private static IEnumerable<IGoapDebugSource> FindAllSources()
        {
            var list = new List<IGoapDebugSource>();
            var behaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IGoapDebugSource source)
                {
                    list.Add(source);
                }
            }

            return list;
        }

        private static IGoapDebugSource FindSourceOnSelection()
        {
            if (Selection.activeGameObject == null)
            {
                return null;
            }

            var behaviours = Selection.activeGameObject.GetComponents<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IGoapDebugSource source)
                {
                    return source;
                }
            }

            return null;
        }

        private static void ReplaceElement(VisualElement placeholder, VisualElement replacement)
        {
            if (placeholder == null || placeholder.parent == null)
            {
                return;
            }

            var parent = placeholder.parent;
            var index = parent.IndexOf(placeholder);
            parent.Remove(placeholder);
            parent.Insert(index, replacement);
        }

        private sealed class AgentOption
        {
            public IGoapDebugSource Source { get; }
            public string Label { get; }

            public AgentOption(IGoapDebugSource source)
            {
                Source = source;
                Label = ResolveLabel(source);
            }

            private static string ResolveLabel(IGoapDebugSource source)
            {
                if (source == null)
                {
                    return "<none>";
                }

                if (source is MonoBehaviour behaviour)
                {
                    if (behaviour == null)
                    {
                        return "<destroyed>";
                    }

                    return behaviour.name;
                }

                return source.ToString();
            }
        }

        private sealed class PlanGraphElement : VisualElement
        {
            private IReadOnlyList<GoapDebugPlanStep> steps = Array.Empty<GoapDebugPlanStep>();
            private int currentIndex = -1;

            public PlanGraphElement()
            {
                style.flexDirection = FlexDirection.Row;
                style.alignItems = Align.Center;
                style.flexGrow = 0f;
                style.flexShrink = 0f;
                generateVisualContent += OnGenerateVisualContent;
            }

            public void SetSteps(IReadOnlyList<GoapDebugPlanStep> newSteps, int newCurrentIndex)
            {
                steps = newSteps ?? Array.Empty<GoapDebugPlanStep>();
                currentIndex = newCurrentIndex;
                Clear();

                for (var i = 0; i < steps.Count; i++)
                {
                    var label = new Label(string.IsNullOrEmpty(steps[i].Name) ? "Step" : steps[i].Name);
                    label.AddToClassList("goap-debugger__graph-node");
                    if (i == currentIndex)
                    {
                        label.AddToClassList("goap-debugger__graph-node--current");
                    }

                    Add(label);
                }

                MarkDirtyRepaint();
            }


            private void OnGenerateVisualContent(MeshGenerationContext context)
            {
                if (steps.Count == 0)
                {
                    return;
                }

                var painter = context.painter2D;
                painter.strokeColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                painter.lineWidth = 2f;

                for (var i = 0; i < childCount - 1; i++)
                {
                    var fromRect = this[i].layout;
                    var toRect = this[i + 1].layout;
                    var from = new Vector2(fromRect.xMax, fromRect.center.y);
                    var to = new Vector2(toRect.xMin, toRect.center.y);

                    painter.BeginPath();
                    painter.MoveTo(from);
                    painter.LineTo(to);
                    painter.Stroke();

                    var arrowSize = 6f;
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(to.x - arrowSize, to.y - arrowSize));
                    painter.LineTo(to);
                    painter.LineTo(new Vector2(to.x - arrowSize, to.y + arrowSize));
                    painter.Stroke();
                }
            }
        }
    }
}


