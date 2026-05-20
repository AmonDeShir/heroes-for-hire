using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.AI.Strategies
{
    public class WanderStrategy<TSnapshot, TAnimationController> : IActionStrategy
        where TSnapshot : IReadOnlyWorldSnapshot

    {
        private readonly Agent<TSnapshot, HeroAnimationController> agent;
        private readonly AgentContext<TSnapshot> context;
        private readonly float radius;
        private readonly Timer timer;
        private bool idleOnly;

        public bool CanPerform => true;
        public bool Complete { get; private set; }

        public WanderStrategy(Agent<TSnapshot, HeroAnimationController> agent, AgentContext<TSnapshot> context,
            float radius
        )
        {
            this.agent = agent;
            this.context = context;
            this.radius = radius;

            timer = new Timer(Random.Range(1.5f, 3.5f), oneShoot: true);
            timer.OnStart += () => Complete = false;
            timer.OnTimeOut += () => Complete = true;
        }

        public void Start()
        {
            idleOnly = Random.value < 0.35f;
            if (!idleOnly)
            {
                var destination = GetRandomNavMeshPoint(agent.transform.position, radius);
                agent.NavAgent.SetDestination(destination);
            }

            timer.Start();
        }

        public void Update(float deltaTime)
        {
            timer.Tick(deltaTime);
            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));

            if (!idleOnly && agent.NavAgent.remainingDistance <= 2f && !agent.NavAgent.pathPending)
            {
                Complete = true;
            }
        }

        public void Stop()
        {
            if (!idleOnly)
            {
                agent.NavAgent.ResetPath();
            }

            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));
        }

        private static Vector3 GetRandomNavMeshPoint(Vector3 origin, float radius)
        {
            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0f;
            var target = origin + randomDirection;

            if (NavMesh.SamplePosition(target, out var hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return origin;
        }
    }
}

