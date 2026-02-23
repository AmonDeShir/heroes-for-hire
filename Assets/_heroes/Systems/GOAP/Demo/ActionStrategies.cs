using Heroes.Animations;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.Systems.GOAP.Demo;
using UnityEngine;

namespace GOAP.Demo.Strategies
{
    public class MoveStrategy<TSnapshot> : IActionStrategy where TSnapshot : IReadOnlyWorldSnapshot {
        private Agent<TSnapshot> agent;
        private AgentContext<TSnapshot> context;
        private readonly Vector3 destination;

        public bool CanPerform => !Complete;
        public bool Complete => agent.NavAgent.remainingDistance <= 2f && !agent.NavAgent.pathPending;
    
        public MoveStrategy(Vector3 destination, Agent<TSnapshot> agent, AgentContext<TSnapshot> context) {
            this.destination = destination;
            this.agent = agent;
            this.context = context; 
        }
    
        public void Start() => agent.NavAgent.SetDestination(destination);
        
        public void Stop()
        {
            agent.NavAgent.ResetPath();
            context.state.SetLocation(destination);
        }

        public void Update(float deltaTime)
        {
            context.state.SetLocation(agent.Rigidbody.position);
        }
    }
    
    
    public class BuyStrategy<TSnapshot> : IActionStrategy where TSnapshot : IReadOnlyWorldSnapshot {
        public bool CanPerform => true; 
        public bool Complete { get; private set; }
    
        private AgentContext<TSnapshot> context;
        
        private readonly int itemToBuy;
        private readonly float price;
        
        private readonly Timer timer;
        private readonly CharacterAnimationController animations;

        public BuyStrategy(CharacterAnimationController animations, AgentContext<TSnapshot> context, int itemToBuy, float price) {
            this.animations = animations;
            this.context = context;
            this.itemToBuy = itemToBuy;
            this.price = price;
            
            timer = new Timer(animations.GetAnimationLength(animations.AttackClip));
            timer.OnStart += () => Complete = false;
            timer.OnTimeOut += () => Complete = true;
        }
    
        public void Start() {
            timer.Start();
            animations.Attack();
        }
    
        public void Update(float deltaTime) => timer.Tick(deltaTime);

        public void Stop()
        {
            context.state.SetBelieve(DemoConsts.GOLD, context.state.GetBelieve(DemoConsts.GOLD) - price);
            context.state.SetBelieve(itemToBuy, context.state.GetBelieve(itemToBuy) + 1);
        }
    }

    public class TimedRewardStrategy<TSnapshot> : IActionStrategy where TSnapshot : IReadOnlyWorldSnapshot
    {
        public bool CanPerform => true;
        public bool Complete { get; private set; }

        private readonly AgentContext<TSnapshot> context;
        private readonly int beliefId;
        private readonly float delta;

        private readonly Timer timer;
        private readonly CharacterAnimationController animations;

        public TimedRewardStrategy(CharacterAnimationController animations, AgentContext<TSnapshot> context, int beliefId, float delta)
        {
            this.animations = animations;
            this.context = context;
            this.beliefId = beliefId;
            this.delta = delta;

            timer = new Timer(animations.GetAnimationLength(animations.AttackClip));
            timer.OnStart += () => Complete = false;
            timer.OnTimeOut += () => Complete = true;
        }

        public void Start()
        {
            timer.Start();
            animations.Attack();
        }

        public void Update(float deltaTime) => timer.Tick(deltaTime);

        public void Stop()
        {
            var value = context.state.GetBelieve(beliefId) + delta;
            context.state.SetBelieve(beliefId, value);
        }
    }

}
