using System;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class MoveStrategy : IActionStrategy
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
        private readonly Func<Vector3> _location;

        public MoveStrategy(NavMeshAgent agent, Func<Vector3> location)
        {
            _agent = agent;
            _location = location;
        }

        public void Start()
        {
            _agent.SetDestination(_location());
        }

        public void Stop()
        {
            _agent.ResetPath();
        }
    }
}