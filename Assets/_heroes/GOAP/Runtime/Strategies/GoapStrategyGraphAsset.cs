using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Goap.Runtime.Strategies
{
    [CreateAssetMenu(menuName = "Heroes/GOAP/Strategy Graph", fileName = "GoapStrategy")]
    public class GoapStrategyGraphAsset : ScriptableObject
    {
        public int EntryNodeId;
        public List<GoapStrategyEdge> Edges = new List<GoapStrategyEdge>();
        [SerializeReference] public List<GoapStrategyNode> Nodes = new List<GoapStrategyNode>();

        public GoapStrategyNode GetNodeById(int id)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Id == id)
                    return Nodes[i];
            }

            return null;
        }

        public int GetNextNodeId(int fromNodeId, string portName)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                var edge = Edges[i];
                if (edge.FromNodeId == fromNodeId && edge.FromPortName == portName)
                    return edge.ToNodeId;
            }

            return -1;
        }
    }

    [System.Serializable]
    public struct GoapStrategyEdge
    {
        public int FromNodeId;
        public string FromPortName;
        public int ToNodeId;
    }
}
