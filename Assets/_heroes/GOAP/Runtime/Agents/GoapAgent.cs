using System;
using System.Collections;
using System.Collections.Generic;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Planner;
using Heroes.Goap.Runtime.Strategies;
using Heroes.Goap.Runtime.World;
using UnityEngine;

namespace Heroes.Goap.Runtime.Agents
{
    public class GoapAgent : MonoBehaviour
    {
        public GoapArchetypeAsset Archetype;
        public GoapStrategyRunner StrategyRunner;
        [SerializeField] GoapNavMeshExecutor NavMeshExecutor;

        public GoapWorldState WorldState { get; } = new GoapWorldState();
        public GoapMemoryState MemoryState { get; } = new GoapMemoryState();

        public GoapPlan CurrentPlan { get; private set; }
        public GoapGoalDefinition CurrentGoal { get; private set; }
        public GoapActionDefinition CurrentAction { get; private set; }
        public GoapStrategyGraphAsset CurrentStrategy { get; private set; }
        public GoapStrategyNode CurrentStrategyNode { get; private set; }
        public string CurrentStrategyPort { get; private set; }
        public IReadOnlyList<GoapGoalDebugInfo> LastGoalDebug => m_GoalDebug;
        public IReadOnlyList<GoapMemoryLogEntry> MemoryLog => m_MemoryLog;
        public IReadOnlyDictionary<string, GoapNodeTimingStats> StrategyNodeTimings => m_NodeTimings;

        readonly GoapPlanner m_Planner = new GoapPlanner();
        readonly List<GoapGoalDebugInfo> m_GoalDebug = new List<GoapGoalDebugInfo>();
        readonly List<GoapMemoryLogEntry> m_MemoryLog = new List<GoapMemoryLogEntry>();
        readonly Dictionary<string, GoapNodeTimingStats> m_NodeTimings = new Dictionary<string, GoapNodeTimingStats>(StringComparer.OrdinalIgnoreCase);
        const int MaxMemoryLogEntries = 100;
        float m_CurrentNodeStartTime;
        string m_CurrentNodeName;

        void Awake()
        {
            if (NavMeshExecutor == null)
                NavMeshExecutor = GetComponent<GoapNavMeshExecutor>();
            ApplyMemoryDefaults();
        }

        void OnEnable()
        {
            if (StrategyRunner != null)
            {
                StrategyRunner.OnNodeStart += HandleNodeStart;
                StrategyRunner.OnNodeEnd += HandleNodeEnd;
            }

            MemoryState.OnValueChanged += HandleMemoryChanged;
        }

        void OnDisable()
        {
            if (StrategyRunner != null)
            {
                StrategyRunner.OnNodeStart -= HandleNodeStart;
                StrategyRunner.OnNodeEnd -= HandleNodeEnd;
            }

            MemoryState.OnValueChanged -= HandleMemoryChanged;
        }

        public GoapPlan Plan()
        {
            CurrentPlan = m_Planner.Plan(Archetype, WorldState, MemoryState, out var bestGoal, out var debug);
            CurrentGoal = bestGoal;

            m_GoalDebug.Clear();
            if (debug != null)
                m_GoalDebug.AddRange(debug);

            return CurrentPlan;
        }

        public IEnumerator RunPlan(GoapPlan plan)
        {
            if (plan == null || StrategyRunner == null)
                yield break;

            CurrentPlan = plan;
            var context = new GoapStrategyContext
            {
                World = WorldState,
                Memory = MemoryState,
                LocationExecutor = NavMeshExecutor,
                WanderExecutor = NavMeshExecutor
            };

            foreach (var action in plan.Actions)
            {
                CurrentAction = action;
                CurrentStrategy = action.Strategy;
                ApplyParameters(action);
                if (action.Strategy == null)
                    continue;

                yield return StrategyRunner.Run(action.Strategy, context);
            }

            CurrentAction = null;
            CurrentStrategy = null;
            CurrentStrategyNode = null;
        }

        void HandleNodeStart(GoapStrategyGraphAsset graph, GoapStrategyNode node)
        {
            CurrentStrategy = graph;
            CurrentStrategyNode = node;
            m_CurrentNodeStartTime = Time.time;
            m_CurrentNodeName = node != null ? node.GetType().Name : string.Empty;
        }

        void HandleNodeEnd(GoapStrategyGraphAsset graph, GoapStrategyNode node, string nextPort)
        {
            CurrentStrategyPort = nextPort;
            if (node != null && !string.IsNullOrWhiteSpace(m_CurrentNodeName))
            {
                var duration = Time.time - m_CurrentNodeStartTime;
                if (!m_NodeTimings.TryGetValue(m_CurrentNodeName, out var stats))
                    stats = new GoapNodeTimingStats();

                stats.AddSample(duration);
                m_NodeTimings[m_CurrentNodeName] = stats;
            }

            if (CurrentStrategyNode == node)
                CurrentStrategyNode = null;
        }

        void HandleMemoryChanged(GoapMemoryChange change)
        {
            m_MemoryLog.Add(new GoapMemoryLogEntry(Time.time, change));
            if (m_MemoryLog.Count > MaxMemoryLogEntries)
                m_MemoryLog.RemoveAt(0);
        }

        void ApplyParameters(GoapActionDefinition action)
        {
            if (action == null || action.Parameters == null)
                return;

            for (int i = 0; i < action.Parameters.Count; i++)
            {
                var parameter = action.Parameters[i];
                if (string.IsNullOrWhiteSpace(parameter.VariableName))
                    continue;

                MemoryState.Set(parameter.VariableName, parameter.Value);
            }
        }

        void ApplyMemoryDefaults()
        {
            if (Archetype == null)
                return;

            var stack = new System.Collections.Generic.Stack<GoapArchetypeAsset>();
            for (var current = Archetype; current != null; current = current.Parent)
                stack.Push(current);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Graph == null)
                    continue;

                foreach (var variable in current.Graph.Variables)
                {
                    if (variable.Scope != Values.GoapVariableScope.Memory)
                        continue;

                    MemoryState.Set(variable.Name, variable.DefaultValue);
                }
            }
        }
    }

    public readonly struct GoapMemoryLogEntry
    {
        public readonly float Time;
        public readonly GoapMemoryChange Change;

        public GoapMemoryLogEntry(float time, GoapMemoryChange change)
        {
            Time = time;
            Change = change;
        }
    }

    public struct GoapNodeTimingStats
    {
        public int Count;
        public float TotalTime;
        public float MaxTime;

        public float Average => Count > 0 ? TotalTime / Count : 0f;

        public void AddSample(float duration)
        {
            Count++;
            TotalTime += duration;
            if (duration > MaxTime)
                MaxTime = duration;
        }
    }
}
