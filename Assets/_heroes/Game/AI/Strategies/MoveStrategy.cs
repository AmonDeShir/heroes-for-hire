using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;

namespace Heroes.Game.AI.Strategies
{
    public class MoveStrategy<TSnapshot, TAnimationController> : IActionStrategy
        where TSnapshot : IReadOnlyWorldSnapshot
        where TAnimationController : IAnimationController {
        
        private Agent<TSnapshot, TAnimationController> agent;
        private AgentContext<TSnapshot> context;
        private readonly Vector3 destination;

        public bool CanPerform => !Complete;
        public bool Complete => agent.NavAgent.remainingDistance <= 2f && !agent.NavAgent.pathPending;
    
        public MoveStrategy(Vector3 destination, Agent<TSnapshot, TAnimationController> agent, AgentContext<TSnapshot> context) {
            this.destination = destination;
            this.agent = agent;
            this.context = context; 
        }
    
        public void Start() => agent.NavAgent.SetDestination(destination);
        
        public void Stop()
        {
            agent.NavAgent.ResetPath();
            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));
        }

        public void Update(float deltaTime)
        {
            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));
        }
    }
}

