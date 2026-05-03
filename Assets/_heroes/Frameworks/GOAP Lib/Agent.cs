using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.GOAP
{
    public abstract class Agent<TSnapshot, TAnimationController> : MonoBehaviour, IGoapDebugSource 
        where TSnapshot : IReadOnlyWorldSnapshot 
        where TAnimationController : IAnimationController
    {
        [SerializeField]
        private TAnimationController animator;

        [SerializeField]
        private NavMeshAgent navAgent;

        [SerializeField]
        private Rigidbody rb;
        
        public TAnimationController Animator => animator;
        public NavMeshAgent NavAgent => navAgent;
        public Rigidbody Rigidbody => rb;
        protected PlanExecutor<Agent<TSnapshot, TAnimationController>, TSnapshot> PlanExecutor => planExecutor;
        
        protected abstract Archetype<Agent<TSnapshot, TAnimationController>, TSnapshot> CreateArchetype();
        protected abstract IWorldState<TSnapshot> CreateWorldState();

        protected Archetype<Agent<TSnapshot, TAnimationController>, TSnapshot> archetype;
        protected IWorldState<TSnapshot> worldState;
        protected IPlanExecutor executor;
        private PlanExecutor<Agent<TSnapshot, TAnimationController>, TSnapshot> planExecutor;
        private GoapDebugAdapter<Agent<TSnapshot, TAnimationController>, TSnapshot> debugAdapter;
        
        public void Start()
        {
            archetype = CreateArchetype();
            worldState = CreateWorldState();
            executor = new PlanExecutor<Agent<TSnapshot, TAnimationController>, TSnapshot>(this, archetype, worldState);
            planExecutor = executor as PlanExecutor<Agent<TSnapshot, TAnimationController>, TSnapshot>;
            
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

            debugAdapter ??= new GoapDebugAdapter<Agent<TSnapshot, TAnimationController>, TSnapshot>(planExecutor);
            return debugAdapter.TryBuildSnapshot(out snapshot);
        }
    }
}
