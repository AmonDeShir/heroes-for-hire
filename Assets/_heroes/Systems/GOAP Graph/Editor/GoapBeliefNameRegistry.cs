using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor
{
    internal static class GoapBeliefNameRegistry
    {
        private sealed class BeliefNamesCache
        {
            public List<string> ValidNames = new List<string>();
        }

        private static readonly ConditionalWeakTable<Graph, BeliefNamesCache> CacheByGraph = new();

        public static void Update(Graph graph, List<string> validNames)
        {
            if (graph == null)
            {
                return;
            }

            var cache = CacheByGraph.GetOrCreateValue(graph);
            cache.ValidNames = validNames ?? new List<string>();
        }

        public static IReadOnlyList<string> GetValidNames(Graph graph)
        {
            if (graph == null)
            {
                return null;
            }

            return CacheByGraph.TryGetValue(graph, out var cache)
                ? cache.ValidNames
                : null;
        }
    }
}
