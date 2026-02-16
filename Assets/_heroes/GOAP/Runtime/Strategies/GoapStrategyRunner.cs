using System.Collections;
using UnityEngine;

namespace Heroes.Goap.Runtime.Strategies
{
    public class GoapStrategyRunner : MonoBehaviour
    {
        public event System.Action<GoapStrategyGraphAsset, GoapStrategyNode> OnNodeStart;
        public event System.Action<GoapStrategyGraphAsset, GoapStrategyNode, string> OnNodeEnd;

        public IEnumerator Run(GoapStrategyGraphAsset graph, GoapStrategyContext context)
        {
            if (graph == null)
                yield break;

            int currentId = graph.EntryNodeId;
            int safety = 0;

            while (currentId >= 0 && safety < 10000)
            {
                safety++;
                var node = graph.GetNodeById(currentId);
                if (node == null)
                    yield break;

                OnNodeStart?.Invoke(graph, node);
                context.NextPort = GoapStrategyPortNames.Out;
                yield return node.Execute(context);
                OnNodeEnd?.Invoke(graph, node, context.NextPort);

                var nextId = graph.GetNextNodeId(currentId, context.NextPort);
                if (nextId < 0)
                    yield break;

                currentId = nextId;
            }
        }
    }
}
