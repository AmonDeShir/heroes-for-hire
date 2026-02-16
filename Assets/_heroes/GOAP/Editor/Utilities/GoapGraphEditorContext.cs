using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Heroes.Goap.Editor.Graphs;
using Heroes.Goap.Editor.Nodes;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Values;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Heroes.Goap.Editor.Utilities
{
    internal static class GoapGraphEditorContext
    {
        public static Unity.GraphToolkit.Editor.Graph GetFocusedGraph()
        {
            var window = EditorWindow.focusedWindow;
            if (window == null)
                return null;

            var graphTool = GetProperty(window, "GraphTool");
            var toolState = GetProperty(graphTool, "ToolState");
            var graphModel = GetProperty(toolState, "GraphModel");
            return GetProperty(graphModel, "Graph") as Unity.GraphToolkit.Editor.Graph;
        }

        public static string GetFocusedGraphPath()
        {
            var window = EditorWindow.focusedWindow;
            if (window == null)
                return null;

            var graphTool = GetProperty(window, "GraphTool");
            var toolState = GetProperty(graphTool, "ToolState");
            var graphModel = GetProperty(toolState, "GraphModel");
            var graphObject = GetProperty(graphModel, "GraphObject");
            return GetProperty(graphObject, "FilePath") as string;
        }

        public static List<string> GetVariableNamesFromFocusedGraph()
        {
            var graph = GetFocusedGraph();
            return GetVariableNames(graph);
        }

        public static System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType> GetVariableTypesFromFocusedGraph()
        {
            var graph = GetFocusedGraph();
            return GetVariableTypes(graph);
        }

        static List<string> GetVariableNames(Unity.GraphToolkit.Editor.Graph graph)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (graph is GoapArchetypeGraph archetypeGraph)
            {
                foreach (var variable in archetypeGraph.GetVariables())
                    names.Add(variable.name);

                var root = archetypeGraph.GetNodes().OfType<ArchetypeRoot_Node>().FirstOrDefault();
                if (root != null)
                {
                    var parent = GoapNodeOptionReader.GetOption<GoapArchetypeAsset>(root, ArchetypeRoot_Node.OptionParent, null);
                    while (parent != null)
                    {
                        if (parent.Graph != null)
                        {
                            foreach (var variable in parent.Graph.Variables)
                                names.Add(variable.Name);
                        }
                        parent = parent.Parent;
                    }
                }
            }
            else if (graph is GoapStrategyGraph)
            {
                var focusedPath = GetFocusedGraphPath();
                if (!string.IsNullOrWhiteSpace(focusedPath) && focusedPath.EndsWith($".{GoapArchetypeGraph.AssetExtension}", StringComparison.OrdinalIgnoreCase))
                {
                    var focusedArchetypeGraph = GraphDatabase.LoadGraph<GoapArchetypeGraph>(focusedPath);
                    if (focusedArchetypeGraph != null)
                    {
                        foreach (var name in GetVariableNames(focusedArchetypeGraph))
                            names.Add(name);
                    }
                }

                if (names.Count == 0)
                {
                    var strategyAsset = LoadStrategyAssetFromFocusedGraph();
                    if (strategyAsset != null)
                    {
                        var graphs = FindGraphsReferencingStrategy(strategyAsset);
                        foreach (var graphAsset in graphs)
                        {
                            foreach (var variable in graphAsset.Variables)
                                names.Add(variable.Name);
                        }
                    }
                }
            }

            return names.OrderBy(n => n).ToList();
        }

        static System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType> GetVariableTypes(Unity.GraphToolkit.Editor.Graph graph)
        {
            var map = new System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType>(StringComparer.OrdinalIgnoreCase);

            if (graph is GoapArchetypeGraph archetypeGraph)
            {
                AddVariables(archetypeGraph.GetVariables(), map);

                var root = archetypeGraph.GetNodes().OfType<ArchetypeRoot_Node>().FirstOrDefault();
                if (root != null)
                {
                    var parent = GoapNodeOptionReader.GetOption<Heroes.Goap.Runtime.Core.GoapArchetypeAsset>(root, ArchetypeRoot_Node.OptionParent, null);
                    while (parent != null)
                    {
                        if (parent.Graph != null)
                            AddVariables(parent.Graph.Variables, map);
                        parent = parent.Parent;
                    }
                }
            }
            else if (graph is GoapStrategyGraph)
            {
                var focusedPath = GetFocusedGraphPath();
                if (!string.IsNullOrWhiteSpace(focusedPath) && focusedPath.EndsWith($".{GoapArchetypeGraph.AssetExtension}", StringComparison.OrdinalIgnoreCase))
                {
                    var focusedArchetypeGraph = GraphDatabase.LoadGraph<GoapArchetypeGraph>(focusedPath);
                    if (focusedArchetypeGraph != null)
                    {
                        var archetypeTypes = GetVariableTypes(focusedArchetypeGraph);
                        foreach (var pair in archetypeTypes)
                            map[pair.Key] = pair.Value;
                    }
                }

                if (map.Count == 0)
                {
                    var strategyAsset = LoadStrategyAssetFromFocusedGraph();
                    if (strategyAsset != null)
                    {
                        var graphs = FindGraphsReferencingStrategy(strategyAsset);
                        foreach (var graphAsset in graphs)
                            AddVariables(graphAsset.Variables, map);
                    }
                }
            }

            return map;
        }

        static void AddVariables(System.Collections.Generic.IEnumerable<Heroes.Goap.Runtime.Values.GoapVariableDef> variables, System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType> map)
        {
            foreach (var variable in variables)
                map[variable.Name] = variable.Type;
        }

        static void AddVariables(System.Collections.Generic.IEnumerable<Unity.GraphToolkit.Editor.IVariable> variables, System.Collections.Generic.Dictionary<string, Heroes.Goap.Runtime.Values.GoapValueType> map)
        {
            foreach (var variable in variables)
            {
                if (variable.dataType == typeof(float))
                    map[variable.name] = Heroes.Goap.Runtime.Values.GoapValueType.Float;
                else if (variable.dataType == typeof(bool))
                    map[variable.name] = Heroes.Goap.Runtime.Values.GoapValueType.Bool;
                else if (variable.dataType == typeof(Heroes.Goap.Runtime.World.LocationSO))
                    map[variable.name] = Heroes.Goap.Runtime.Values.GoapValueType.Location;
            }
        }

        static object GetProperty(object target, string name)
        {
            if (target == null)
                return null;

            var property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return property?.GetValue(target);
        }

        static Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset LoadStrategyAssetFromFocusedGraph()
        {
            var path = GetFocusedGraphPath();
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return UnityEditor.AssetDatabase.LoadAssetAtPath<Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset>(path);
        }

        static List<Heroes.Goap.Runtime.Core.GoapGraphAsset> FindGraphsReferencingStrategy(Heroes.Goap.Runtime.Strategies.GoapStrategyGraphAsset strategy)
        {
            var results = new List<Heroes.Goap.Runtime.Core.GoapGraphAsset>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:GoapGraphAsset");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var graph = UnityEditor.AssetDatabase.LoadAssetAtPath<Heroes.Goap.Runtime.Core.GoapGraphAsset>(path);
                if (graph == null)
                    continue;

                for (int i = 0; i < graph.Actions.Count; i++)
                {
                    if (graph.Actions[i].Strategy == strategy)
                    {
                        results.Add(graph);
                        break;
                    }
                }
            }

            return results;
        }
    }
}
