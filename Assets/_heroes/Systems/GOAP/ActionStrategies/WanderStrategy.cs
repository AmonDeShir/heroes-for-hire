using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace GOAP
{
    public class WanderStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;

        public bool Complete
        {
            get
            {
                return _agent.remainingDistance <= 2f && !_agent.pathPending;
            }
        }
        
        private readonly NavMeshAgent _agent;
        private readonly float _wanderRadius;

        public WanderStrategy(NavMeshAgent agent, float wanderRadius)
        {
            _agent = agent;
            _wanderRadius = wanderRadius;
        }

        public void Start()
        {
            for (var i = 0; i < 5; i++)
            {
                Vector3 randomDirection = (Random.insideUnitCircle * _wanderRadius).With(y: 0);
                NavMeshHit hit;

                if (NavMesh.SamplePosition(_agent.transform.position + randomDirection, out hit, _wanderRadius, 1))
                {
                    _agent.SetDestination(hit.position);
                    
                    return;
                }
            }
        }
    }
}