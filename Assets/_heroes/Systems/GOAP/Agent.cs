using Heroes.Animations;
using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.GOAP
{
    public abstract class Agent<TSnapshot> : MonoBehaviour, IGoapDebugSource where TSnapshot : IReadOnlyWorldSnapshot
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
        
        protected abstract Archetype<Agent<TSnapshot>, TSnapshot> CreateArchetype();
        protected abstract IWorldState<TSnapshot> CreateWorldState();

        protected Archetype<Agent<TSnapshot>, TSnapshot> archetype;
        protected IWorldState<TSnapshot> worldState;
        protected IPlanExecutor executor;
        private PlanExecutor<Agent<TSnapshot>, TSnapshot> planExecutor;
        private GoapDebugAdapter<Agent<TSnapshot>, TSnapshot> debugAdapter;
        
        public void Start()
        {
            archetype = CreateArchetype();
            worldState = CreateWorldState();
            executor = new PlanExecutor<Agent<TSnapshot>, TSnapshot>(this, archetype, worldState);
            planExecutor = executor as PlanExecutor<Agent<TSnapshot>, TSnapshot>;
            
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

        public bool TryGetSnapshot(out GoapDebugSnapshot snapshot)
        {
            if (planExecutor == null)
            {
                snapshot = null;
                return false;
            }

            debugAdapter ??= new GoapDebugAdapter<Agent<TSnapshot>, TSnapshot>(planExecutor);
            return debugAdapter.TryBuildSnapshot(out snapshot);
        }
    }
}
