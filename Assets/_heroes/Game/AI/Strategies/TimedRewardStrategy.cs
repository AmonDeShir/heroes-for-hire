using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public class TimedRewardStrategy<TSnapshot> : IActionStrategy where TSnapshot : IReadOnlyWorldSnapshot
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private readonly AgentContext<TSnapshot> context;
        private readonly int beliefId;
        private readonly float delta;

        private readonly Timer timer;
        private readonly HeroAnimationController animations;
        private const float MinDurationSeconds = 1f;

        public TimedRewardStrategy(HeroAnimationController animations, AgentContext<TSnapshot> context, int beliefId, float delta)
        {
            this.animations = animations;
            this.context = context;
            this.beliefId = beliefId;
            this.delta = delta;

            timer = new Timer(ResolveDuration(animations), oneShoot: true);
            timer.OnStart += () => Complete = false;
            timer.OnTimeOut += () => Complete = true;
        }

        public void Start()
        {
            timer.Start();
            animations.PlayAttack();
        }

        public void Update(float deltaTime) => timer.Tick(deltaTime);

        public void Stop()
        {
            context.MutateState((ref AgentState s) =>
            {
                var value = s.GetBelieve(beliefId) + delta;
                s.SetBelieve(beliefId, value);
            });
        }

        private static float ResolveDuration(HeroAnimationController animations)
        {
            var length = animations.GetAnimationLength(animations.AttackClip);
            return length > 0f ? length : MinDurationSeconds;
        }
    }
}