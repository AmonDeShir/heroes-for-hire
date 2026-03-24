using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Heroes.Systems.GOAPGraph.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class GOAPGraph : Graph
    {
        public const string AssetExtension = "goap";    
    
        [MenuItem("Assets/Create/GOAP Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GOAPGraph>();
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);

            var beliefNames = new Dictionary<string, List<Nodes.DefineBeliefNode>>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in GetNodes().OfType<Nodes.DefineBeliefNode>())
            {
                var name = GetNodeOptionValue(node, Nodes.DefineBeliefNode.NameOptionId);
                if (string.IsNullOrWhiteSpace(name))
                {
                    graphLogger.LogError("Belief name is required.", node);
                    continue;
                }

                if (!beliefNames.TryGetValue(name, out var nodes))
                {
                    nodes = new List<Nodes.DefineBeliefNode>();
                    beliefNames.Add(name, nodes);
                }

                nodes.Add(node);
            }

            foreach (var entry in beliefNames)
            {
                if (entry.Value.Count <= 1)
                {
                    continue;
                }

                foreach (var node in entry.Value)
                {
                    graphLogger.LogError($"Duplicate belief name '{entry.Key}'.", node);
                }
            }

            var validBeliefNames = beliefNames
                .Where(entry => entry.Value.Count == 1)
                .Select(entry => entry.Key)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            GoapBeliefNameRegistry.Update(this, validBeliefNames);

        }

        private static string GetNodeOptionValue(Node node, string optionId)
        {
            if (node == null || string.IsNullOrWhiteSpace(optionId))
            {
                return string.Empty;
            }

            var option = node.GetNodeOptionByName(optionId);
            if (option != null && option.TryGetValue(out string value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}
