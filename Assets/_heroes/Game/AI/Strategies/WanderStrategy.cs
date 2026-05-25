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
        private Vector3 destination;
        private float nextRerollAt;
        private float lastProgressAt;
        private float lastRemaining = float.MaxValue;

        public bool CanPerform => true;
        public bool Complete { get; private set; }

        public WanderStrategy(Agent<TSnapshot, HeroAnimationController> agent, AgentContext<TSnapshot> context,
            float radius
        )
        {
            this.agent = agent;
            this.context = context;
            this.radius = radius;
        }

        public void Start()
        {
            Complete = false;
            var now = Time.unscaledTime;
            nextRerollAt = now + 5f;
            lastProgressAt = now;
            lastRemaining = float.MaxValue;

            destination = GetRandomNavMeshPoint(agent.transform.position, radius);
            agent.NavAgent.SetDestination(destination);
        }

        public void Update(float deltaTime)
        {
            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));

            if (Complete || agent == null || agent.NavAgent == null)
            {
                Complete = true;
                return;
            }

            if (agent.NavAgent.pathPending)
            {
                return;
            }

            var now = Time.unscaledTime;
            var remaining = agent.NavAgent.remainingDistance;

            if (remaining + 0.05f < lastRemaining)
            {
                lastRemaining = remaining;
                lastProgressAt = now;
            }

            var reachThreshold = Mathf.Max(0.5f, agent.NavAgent.stoppingDistance + 0.25f);
            var reached = agent.NavAgent.hasPath && !float.IsInfinity(remaining) && remaining <= reachThreshold;
            var stalled = now >= nextRerollAt || (now - lastProgressAt) >= 5f;

            if (reached || stalled || !agent.NavAgent.hasPath || float.IsInfinity(remaining))
            {
                destination = GetRandomNavMeshPoint(agent.transform.position, radius);
                agent.NavAgent.SetDestination(destination);
                nextRerollAt = now + 5f;
                lastProgressAt = now;
                lastRemaining = float.MaxValue;
            }
        }

        public void Stop()
        {
            agent.NavAgent.ResetPath();

            context.MutateState((ref AgentState s) => s.SetLocation(agent.transform.position));
        }

        private static Vector3 GetRandomNavMeshPoint(Vector3 origin, float radius)
        {
            var terrains = Terrain.activeTerrains;
            if (terrains != null && terrains.Length > 0)
            {
                var pick = PickRandomTerrain(terrains);
                if (pick != null && pick.terrainData != null)
                {
                    var tp = pick.GetPosition();
                    var size = pick.terrainData.size;
                    var terrainTarget = new Vector3(
                        tp.x + Random.value * size.x,
                        0f,
                        tp.z + Random.value * size.z
                    );

                    terrainTarget.y = pick.SampleHeight(terrainTarget) + tp.y;

                    if (NavMesh.SamplePosition(terrainTarget, out var terrainHit, 200f, NavMesh.AllAreas))
                    {
                        return terrainHit.position;
                    }
                }
            }

            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0f;
            var localTarget = origin + randomDirection;

            if (NavMesh.SamplePosition(localTarget, out var localHit, radius, NavMesh.AllAreas))
            {
                return localHit.position;
            }

            return origin;
        }

        private static Terrain PickRandomTerrain(Terrain[] terrains)
        {
            if (terrains == null || terrains.Length == 0)
            {
                return null;
            }

            var total = 0f;
            for (var i = 0; i < terrains.Length; i++)
            {
                var t = terrains[i];
                if (t == null || t.terrainData == null)
                {
                    continue;
                }

                var s = t.terrainData.size;
                total += Mathf.Max(0.001f, s.x * s.z);
            }

            if (total <= 0.001f)
            {
                return terrains[0];
            }

            var roll = Random.value * total;
            for (var i = 0; i < terrains.Length; i++)
            {
                var t = terrains[i];
                if (t == null || t.terrainData == null)
                {
                    continue;
                }

                var s = t.terrainData.size;
                roll -= Mathf.Max(0.001f, s.x * s.z);
                if (roll <= 0f)
                {
                    return t;
                }
            }

            return terrains[0];
        }
    }
}

