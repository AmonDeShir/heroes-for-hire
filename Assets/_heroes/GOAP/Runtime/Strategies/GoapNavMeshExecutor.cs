using System.Collections;
using Heroes.Goap.Runtime.World;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Goap.Runtime.Strategies
{
    public class GoapNavMeshExecutor : MonoBehaviour, IGoapLocationExecutor, IGoapWanderExecutor
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] AnimationController animationController;
        [SerializeField] float arrivalDistance = 2f;

        void Awake()
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
            if (animationController == null)
                animationController = GetComponent<AnimationController>();
        }

        public IEnumerator MoveTo(LocationSO location)
        {
            if (agent == null || location == null)
                yield break;

            if (!GoapWorldState.TryGetClosestLocation(location, transform.position, out var marker))
                yield break;

            agent.SetDestination(marker.transform.position);
            while (!HasArrived())
            {
                UpdateAnimationSpeed();
                yield return null;
            }

            UpdateAnimationSpeed();
        }

        public IEnumerator Wander(float radius)
        {
            if (agent == null)
                yield break;

            var origin = transform.position;
            for (var i = 0; i < 5; i++)
            {
                var randomDirection = Random.insideUnitCircle * radius;
                var target = origin + new Vector3(randomDirection.x, 0f, randomDirection.y);
                if (NavMesh.SamplePosition(target, out var hit, radius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    break;
                }
            }

            while (!HasArrived())
            {
                UpdateAnimationSpeed();
                yield return null;
            }

            UpdateAnimationSpeed();
        }

        bool HasArrived()
        {
            if (agent.pathPending)
                return false;

            return agent.remainingDistance <= arrivalDistance;
        }

        void UpdateAnimationSpeed()
        {
            if (animationController != null)
                animationController.SetSpeed(agent.velocity.magnitude);
        }
    }
}
