using Heroes.Animations;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.GOAP
{
    public abstract class Agent : MonoBehaviour
    {
        [SerializeField]
        private CharacterAnimationController animator;

        [SerializeField]
        private NavMeshAgent navAgent;

        [SerializeField]
        private Rigidbody rb;
        
        public CharacterAnimationController Animator => animator;
        public NavMeshAgent NavAgent => navAgent;
        public Rigidbody Rigidbody => rb;
        
        protected abstract Archetype<Agent> CreateArchetype();
        protected Archetype<Agent> archetype;
        protected PlanExecutor<Agent> executor;
        
        protected void Awake()
        {
            archetype = CreateArchetype();
            executor = new PlanExecutor<Agent>(this, archetype);
        }

        public void Start()
        {
            executor.OnNextStepLoaded += ResetNavPath;
        }

        protected void OnDestroy()
        {
            executor.OnNextStepLoaded -= ResetNavPath;
        }

        protected void ResetNavPath()
        {
            navAgent.ResetPath();
        }

        public void Update()
        {
            animator.SetSpeed(navAgent.velocity.magnitude);
            executor.Update(Time.deltaTime);
        }
    }
}