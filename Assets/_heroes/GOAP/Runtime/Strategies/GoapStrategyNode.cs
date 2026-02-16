using System.Collections;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public abstract class GoapStrategyNode
    {
        public int Id;

        public abstract IEnumerator Execute(GoapStrategyContext context);
    }
}
