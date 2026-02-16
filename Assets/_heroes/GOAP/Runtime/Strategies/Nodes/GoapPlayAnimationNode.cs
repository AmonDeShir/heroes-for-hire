using System.Collections;

namespace Heroes.Goap.Runtime.Strategies
{
    [System.Serializable]
    public class GoapPlayAnimationNode : GoapStrategyNode
    {
        public string AnimationName;

        public override IEnumerator Execute(GoapStrategyContext context)
        {
            if (context.Animation != null)
                yield return context.Animation.PlayAnimation(AnimationName);

            context.NextPort = GoapStrategyPortNames.Out;
        }
    }
}
