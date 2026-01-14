#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GOAP.Editor
{
    public class GoapDebuggerWindow : EditorWindow
    {
        private GoapAgent _agent;
        private Vector2 _scroll;

        private bool _autoRefresh = true;
        private bool _showOnlyTrueBeliefs = false;

        private bool _foldStats = true;
        private bool _foldPlan = true;
        private bool _foldBeliefs = true;
        private bool _foldActions = true;

        private bool _showPlanGraph = true;

        private double _lastRepaint;
        private const double RepaintHz = 10.0;

        [MenuItem("Tools/GOAP/Debugger")]
        public static void Open()
        {
            var w = GetWindow<GoapDebuggerWindow>();
            w.titleContent = new GUIContent("GOAP Debugger");
            w.Show();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += HandleSelectionChanged;
            EditorApplication.update += HandleEditorUpdate;
            HandleSelectionChanged();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= HandleSelectionChanged;
            EditorApplication.update -= HandleEditorUpdate;
        }

        private void HandleSelectionChanged()
        {
            _agent = null;

            if (Selection.activeGameObject != null)
            {
                _agent = Selection.activeGameObject.GetComponentInParent<GoapAgent>();
            }

            Repaint();
        }

        private void HandleEditorUpdate()
        {
            if (!_autoRefresh)
            {
                return;
            }

            var now = EditorApplication.timeSinceStartup;
            if (now - _lastRepaint < 1.0 / RepaintHz)
            {
                return;
            }

            _lastRepaint = now;
            Repaint();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (_agent == null)
            {
                EditorGUILayout.HelpBox("Select GOAP Agent.", MessageType.Info);
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawAgentHeader(_agent);

            EditorGUILayout.Space(8);

            _foldStats = EditorGUILayout.Foldout(_foldStats, "Stats", true);
            if (_foldStats)
            {
                DrawStatsPanel(_agent);
            }

            EditorGUILayout.Space(8);

            _foldPlan = EditorGUILayout.Foldout(_foldPlan, "Current Plan", true);
            if (_foldPlan)
            {
                DrawPlanPanel(_agent);
            }

            EditorGUILayout.Space(8);

            _foldBeliefs = EditorGUILayout.Foldout(_foldBeliefs, "Beliefs", true);
            if (_foldBeliefs)
            {
                DrawBeliefsPanel(_agent);
            }

            EditorGUILayout.Space(8);

            _foldActions = EditorGUILayout.Foldout(_foldActions, "Actions", true);
            if (_foldActions)
            {
                DrawActionsPanel(_agent);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto", EditorStyles.toolbarButton, GUILayout.Width(45));
                _showOnlyTrueBeliefs = GUILayout.Toggle(_showOnlyTrueBeliefs, "Only true beliefs", EditorStyles.toolbarButton, GUILayout.Width(120));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ping", EditorStyles.toolbarButton, GUILayout.Width(40)) && _agent != null)
                {
                    EditorGUIUtility.PingObject(_agent.gameObject);
                }
            }
        }

        private static void DrawAgentHeader(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(agent.name, EditorStyles.boldLabel);

                var goal = agent.CurrentGoal != null ? agent.CurrentGoal.Name : "(none)";
                var action = agent.CurrentAction != null ? agent.CurrentAction.Name : "(none)";
                var planCount = TryGetPlanCount(agent);

                EditorGUILayout.LabelField("Goal", goal);
                EditorGUILayout.LabelField("Action", action);
                EditorGUILayout.LabelField("Plan actions left", planCount >= 0 ? planCount.ToString() : "(unknown)");
            }
        }

        private static void DrawStatsPanel(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                var stats = ReadStats(agent);

                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawKV("Health", stats.HealthStr, 140);
                    DrawKV("Stamina", stats.StaminaStr, 140);
                    DrawKV("Gold", stats.GoldStr, 140);
                    DrawKV("HasSword", stats.HasSwordStr, 140);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (stats.HasPickaxeKnown)
                    {
                        DrawKV("HasPickaxe", stats.HasPickaxeStr, 140);
                    }
                    if (stats.CoffeeKnown)
                    {
                        DrawKV("Coffee", stats.CoffeeStr, 140);
                    }
                }

                var nav = agent.GetComponent<UnityEngine.AI.NavMeshAgent>();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (nav != null)
                    {
                        DrawKV("Speed", nav.velocity.magnitude.ToString("0.00"), 140);
                    }
                    DrawKV("Pos", $"{agent.transform.position.x:0.0},{agent.transform.position.y:0.0},{agent.transform.position.z:0.0}", 280);
                }
            }
        }

        private static void DrawKV(string k, string v, float width)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(width)))
            {
                EditorGUILayout.LabelField(k, EditorStyles.miniLabel);
                EditorGUILayout.LabelField(v);
            }
        }

        private void DrawPlanPanel(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Steps", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    _showPlanGraph = EditorGUILayout.ToggleLeft("Graph", _showPlanGraph, GUILayout.Width(70));
                }

                var steps = BuildExecutionSteps(agent);

                if (steps.Count == 0)
                {
                    EditorGUILayout.LabelField("(no plan)");
                    return;
                }

                for (var i = 0; i < steps.Count; i++)
                {
                    var prefix = i == 0 && agent.CurrentAction != null ? "NOW" : (i == 0 ? "NEXT" : $"#{i}");
                    EditorGUILayout.LabelField($"{prefix}: {steps[i]}");
                }

                if (_showPlanGraph)
                {
                    GUILayout.Space(8);
                    var rect = GUILayoutUtility.GetRect(10, 180, GUILayout.ExpandWidth(true));
                    DrawPlanGraph(rect, agent, steps);
                }
            }
        }

        private void DrawBeliefsPanel(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (agent.Beliefs == null)
                {
                    EditorGUILayout.LabelField("(null)");
                    return;
                }

                var list = agent.Beliefs.Values
                    .OrderBy(b => b.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(b => (belief: b, value: SafeEval(b)))
                    .Where(x => !_showOnlyTrueBeliefs || x.value)
                    .ToList();

                if (list.Count == 0)
                {
                    EditorGUILayout.LabelField("(none)");
                    return;
                }

                foreach (var (belief, value) in list)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var style = new GUIStyle(EditorStyles.label)
                        {
                            normal =
                            {
                                textColor = value
                                    ? new Color(0.25f, 0.75f, 0.35f)
                                    : new Color(0.85f, 0.35f, 0.35f)
                            }
                        };

                        EditorGUILayout.LabelField(value ? "TRUE" : "FALSE", style, GUILayout.Width(48));
                        EditorGUILayout.LabelField(belief.Name);

                        var loc = belief.Location;
                        if (loc != Vector3.zero)
                        {
                            EditorGUILayout.LabelField($"({loc.x:0.0},{loc.y:0.0},{loc.z:0.0})", GUILayout.Width(170));
                        }
                    }
                }
            }
        }

        private void DrawActionsPanel(GoapAgent agent)
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (agent.actions == null || agent.actions.Count == 0)
                {
                    EditorGUILayout.LabelField("(none)");
                    return;
                }

                foreach (var a in agent.actions.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
                {
                    using (new EditorGUILayout.VerticalScope("box"))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField(a.Name, EditorStyles.boldLabel);

                            GUILayout.FlexibleSpace();

                            var can = CanActionRunNow(a);
                            var style = new GUIStyle(EditorStyles.label)
                            {
                                normal =
                                {
                                    textColor = can
                                        ? new Color(0.25f, 0.75f, 0.35f)
                                        : new Color(0.85f, 0.35f, 0.35f)
                                }
                            };

                            EditorGUILayout.LabelField(can ? "AVAILABLE" : "BLOCKED", style, GUILayout.Width(80));
                        }

                        DrawBeliefSet("Preconditions", a.Preconditions);
                        DrawBeliefSet("Effects", a.Effects);
                    }
                }
            }
        }

        private static void DrawBeliefSet(string title, HashSet<AgentBelief> set)
        {
            if (set == null || set.Count == 0)
            {
                EditorGUILayout.LabelField($"{title}: (none)");
                return;
            }

            EditorGUILayout.LabelField($"{title}:");
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var b in set.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
                {
                    EditorGUILayout.LabelField($"- {b.Name}");
                }
            }
        }

        private static bool SafeEval(AgentBelief b)
        {
            try { return b.Evaluate(); }
            catch { return false; }
        }

        private static bool CanActionRunNow(AgentAction a)
        {
            if (a == null)
            {
                return false;
            }
            if (a.Preconditions == null || a.Preconditions.Count == 0)
            {
                return true;
            }

            foreach (var p in a.Preconditions)
            {
                if (!SafeEval(p))
                {
                    return false;
                }
            }

            return true;
        }

        private static int TryGetPlanCount(GoapAgent agent)
        {
            if (agent.ActionPlan == null)
            {
                return 0;
            }

            var actionsObj = GetMemberValue(agent.ActionPlan, "Actions");
            if (actionsObj == null)
            {
                return -1;
            }

            if (actionsObj is ICollection col)
            {
                return col.Count;
            }

            var countProp = actionsObj.GetType().GetProperty("Count", BindingFlags.Instance | BindingFlags.Public);
            if (countProp != null && countProp.PropertyType == typeof(int))
            {
                return (int)countProp.GetValue(actionsObj);
            }

            return -1;
        }

        private static List<string> BuildExecutionSteps(GoapAgent agent)
        {
            var steps = new List<string>();

            if (agent.CurrentAction != null)
            {
                steps.Add(agent.CurrentAction.Name);
            }

            var remaining = GetPlanActionsInExecutionOrder(agent.ActionPlan);
            foreach (var a in remaining)
            {
                steps.Add(a.Name);
            }

            return steps;
        }

        private static List<AgentAction> GetPlanActionsInExecutionOrder(ActionPlan plan)
        {
            var result = new List<AgentAction>();
            if (plan == null)
            {
                return result;
            }

            var actionsObj = GetMemberValue(plan, "Actions");
            if (actionsObj == null)
            {
                return result;
            }

            if (actionsObj is IEnumerable enumerable)
            {
                foreach (var x in enumerable)
                {
                    if (x is AgentAction aa)
                    {
                        result.Add(aa);
                    }
                }
            }

            result.Reverse();
            return result;
        }

        private static object GetMemberValue(object obj, string name)
        {
            if (obj == null)
            {
                return null;
            }

            var t = obj.GetType();

            var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null)
            {
                return f.GetValue(obj);
            }

            var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null)
            {
                return p.GetValue(obj);
            }

            return null;
        }

        private void DrawPlanGraph(Rect rect, GoapAgent agent, List<string> steps)
        {
            EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f));
            Handles.BeginGUI();

            var margin = 10f;
            var nodeW = 150f;
            var nodeH = 34f;
            var gap = 26f;

            var nodes = new List<(Rect r, string label, bool active)>();

            var goalName = agent.CurrentGoal != null ? agent.CurrentGoal.Name : (agent.ActionPlan != null ? SafeGetPlanGoalName(agent.ActionPlan) : "Goal");
            var goalRect = new Rect(rect.x + margin, rect.y + margin, nodeW, nodeH);
            nodes.Add((goalRect, $"GOAL: {goalName}", true));

            var x = goalRect.xMax + gap;
            var y = rect.y + margin;

            for (var i = 0; i < steps.Count; i++)
            {
                var isNow = agent.CurrentAction != null && i == 0;
                var r = new Rect(x, y, nodeW, nodeH);
                var label = isNow ? $"NOW: {steps[i]}" : steps[i];
                nodes.Add((r, label, isNow));
                x += nodeW + gap;
            }

            for (var i = 0; i < nodes.Count - 1; i++)
            {
                var a = nodes[i].r;
                var b = nodes[i + 1].r;

                var start = new Vector2(a.xMax, a.center.y);
                var end = new Vector2(b.xMin, b.center.y);
                var tanA = start + Vector2.right * 40f;
                var tanB = end + Vector2.left * 40f;

                Handles.color = new Color(0.8f, 0.8f, 0.8f);
                Handles.DrawBezier(start, end, tanA, tanB, Handles.color, null, 2f);
            }

            foreach (var (r, label, active) in nodes)
            {
                var bg = active ? new Color(0.25f, 0.55f, 0.9f) : new Color(0.22f, 0.22f, 0.22f);
                EditorGUI.DrawRect(r, bg);

                var style = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white },
                    clipping = TextClipping.Clip
                };

                GUI.Label(r, label, style);
            }

            Handles.EndGUI();
        }

        private static string SafeGetPlanGoalName(ActionPlan plan)
        {
            try
            {
                var goalObj = GetMemberValue(plan, "AgentGoal");
                if (goalObj is AgentGoal g)
                {
                    return g.Name;
                }

                var prop = plan.GetType().GetProperty("AgentGoal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                {
                    var v = prop.GetValue(plan);
                    if (v is AgentGoal gg)
                    {
                        return gg.Name;
                    }
                }
            }
            catch { }

            return "(unknown)";
        }

        private readonly struct AgentStats
        {
            public readonly string HealthStr;
            public readonly string StaminaStr;
            public readonly string GoldStr;
            public readonly string HasSwordStr;

            public readonly bool HasPickaxeKnown;
            public readonly string HasPickaxeStr;

            public readonly bool CoffeeKnown;
            public readonly string CoffeeStr;

            public AgentStats(
                string healthStr,
                string staminaStr,
                string goldStr,
                string hasSwordStr,
                bool hasPickaxeKnown,
                string hasPickaxeStr,
                bool coffeeKnown,
                string coffeeStr)
            {
                HealthStr = healthStr;
                StaminaStr = staminaStr;
                GoldStr = goldStr;
                HasSwordStr = hasSwordStr;
                HasPickaxeKnown = hasPickaxeKnown;
                HasPickaxeStr = hasPickaxeStr;
                CoffeeKnown = coffeeKnown;
                CoffeeStr = coffeeStr;
            }
        }

        private static AgentStats ReadStats(GoapAgent agent)
        {
            var health = agent.Health.ToString("0");
            var stamina = agent.Stamina.ToString("0");
            var gold = agent.Gold.ToString();
            var hasSword = agent.HasSword ? "YES" : "NO";

            var hasPickaxeKnown = TryReadBool(agent, "HasPickaxe", out var hasPickaxe);
            var coffeeKnown = TryReadInt(agent, "Coffee", out var coffee);

            return new AgentStats(
                health,
                stamina,
                gold,
                hasSword,
                hasPickaxeKnown,
                hasPickaxeKnown ? (hasPickaxe ? "YES" : "NO") : "",
                coffeeKnown,
                coffeeKnown ? coffee.ToString() : ""
            );
        }

        private static bool TryReadBool(object obj, string memberName, out bool value)
        {
            value = default;
            var v = GetMemberValue(obj, memberName);
            if (v is bool b)
            {
                value = b;
                return true;
            }
            return false;
        }

        private static bool TryReadInt(object obj, string memberName, out int value)
        {
            value = default;
            var v = GetMemberValue(obj, memberName);
            if (v is int i)
            {
                value = i;
                return true;
            }
            return false;
        }
    }
}
#endif
