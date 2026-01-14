using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AnimationController), typeof(Rigidbody))]
    public class GoapAgent : MonoBehaviour
    {
        [Header("Known Locations")]
        [SerializeField] private Transform homePosition;
        [SerializeField] private Transform workPosition;
        [SerializeField] private Transform minePosition;
        [SerializeField] private Transform shopPosition;

        [Header("Economy")]
        [SerializeField] private int startingGold = 0;

        [SerializeField] private int swordCost = 30;

        [SerializeField] private int pickaxeCost = 25;
        [SerializeField] private int coffeeCost = 8;

        [SerializeField] private int workGoldPerTick = 10;
        [SerializeField] private int mineGoldPerTick = 14;

        [SerializeField] private float workStaminaCost = 18f;
        [SerializeField] private float mineStaminaCost = 26f;

        [SerializeField] private float pickaxeGoldMultiplier = 1.6f;
        [SerializeField] private float pickaxeStaminaMultiplier = 0.85f;

        [SerializeField] private float coffeeStaminaGain = 22f;
        [SerializeField] private float coffeeMaxCap = 55f;

        [Header("Stats")]
        public float Health = 100;
        public float Stamina = 100;

        [Header("Runtime State")]
        public int Gold = 0;
        public bool HasSword = false;
        public bool HasPickaxe = false;
        public int Coffee = 0;

        private NavMeshAgent _navMeshAgent;
        private AnimationController _animations;
        private Rigidbody _rigidbody;
        private Timer _statsTimer;

        private AgentGoal lastGoal;
        public AgentGoal CurrentGoal;
        public AgentAction CurrentAction;
        public ActionPlan ActionPlan;
        public Dictionary<string, AgentBelief> Beliefs;
        public HashSet<AgentAction> actions;
        public HashSet<AgentGoal> goals;

        private IGoapPlanner _planner;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animations = GetComponent<AnimationController>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;

            _planner = new GoapPlanner();
        }

        private void Start()
        {
            Gold = Mathf.Max(0, startingGold);

            SetupTimers();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
        }

        private void SetupTimers()
        {
            _statsTimer = new Timer(2f);
            _statsTimer.OnTimeOut += UpdateStats;
            _statsTimer.Start();
        }

        private void UpdateStats()
        {
            if (homePosition != null && IsInRangeOf(homePosition.position, 3f))
            {
                Stamina += 25;
            }

            Stamina = Mathf.Clamp(Stamina, 0f, 100f);
            Health = Mathf.Clamp(Health, 0f, 100f);
        }

        private bool IsInRangeOf(Vector3 pos, float range)
        {
            return Vector3.Distance(transform.position, pos) <= range;
        }

        private void SetupBeliefs()
        {
            Beliefs = new Dictionary<string, AgentBelief>();
            var factory = new BeliefFactory(this, Beliefs);

            factory.AddBelief(Consts.Beliefs.NOTHING, () => false);

            factory.AddBelief(Consts.Beliefs.AGENT_IDLE, () => !_navMeshAgent.hasPath);
            factory.AddBelief(Consts.Beliefs.AGENT_MOVING, () => _navMeshAgent.hasPath);

            factory.AddBelief(Consts.Beliefs.AGENT_IS_TIRED, () => Stamina <= 15f);
            factory.AddBelief(Consts.Beliefs.AGENT_IS_RESTED, () => Stamina >= 60f);
            factory.AddBelief(Consts.Beliefs.AGENT_STAMINA_OK, () => Stamina >= 35f);

            if (homePosition != null)
            {
                factory.AddLocationBelief(Consts.Beliefs.AGENT_AT_HOME, 3f, homePosition);
            }
            if (workPosition != null)
            {
                factory.AddLocationBelief(Consts.Beliefs.AGENT_AT_WORK, 3f, workPosition);
            }
            if (minePosition != null)
            {
                factory.AddLocationBelief(Consts.Beliefs.AGENT_AT_MINE, 3f, minePosition);
            }
            if (shopPosition != null)
            {
                factory.AddLocationBelief(Consts.Beliefs.AGENT_AT_SHOP, 3f, shopPosition);
            }

            factory.AddBelief(Consts.Beliefs.HAS_SWORD, () => HasSword);
            factory.AddBelief(Consts.Beliefs.HAS_PICKAXE, () => HasPickaxe);
            factory.AddBelief(Consts.Beliefs.HAS_COFFEE, () => Coffee > 0);

            factory.AddBelief(Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_SWORD, () => Gold >= swordCost);
            factory.AddBelief(Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_PICKAXE, () => Gold >= pickaxeCost);
            factory.AddBelief(Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_COFFEE, () => Gold >= coffeeCost);
        }

        private void SetupActions()
        {
            actions = new HashSet<AgentAction>();

            actions.Add(new AgentAction.Builder(Consts.Actions.RELAX)
                .WithStrategy(new IdleStrategy(2))
                .WithEffect(Beliefs[Consts.Beliefs.NOTHING])
                .Build());

            actions.Add(new AgentAction.Builder(Consts.Actions.WANDER_AROUND)
                .WithStrategy(new WanderStrategy(_navMeshAgent, 5))
                .WithEffect(Beliefs[Consts.Beliefs.AGENT_MOVING])
                .Build());

            if (homePosition != null)
            {
                actions.Add(new AgentAction.Builder(Consts.Actions.GO_HOME)
                    .WithCost(1)
                    .WithStrategy(new MoveStrategy(_navMeshAgent, () => homePosition.position))
                    .WithEffect(Beliefs[Consts.Beliefs.AGENT_AT_HOME])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.REST_AT_HOME)
                    .WithCost(1)
                    .WithStrategy(new RestAtHomeStrategy(this, 5f, 60f))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_HOME])
                    .WithEffect(Beliefs[Consts.Beliefs.AGENT_IS_RESTED])
                    .Build());
            }

            if (workPosition != null)
            {
                actions.Add(new AgentAction.Builder(Consts.Actions.GO_WORK)
                    .WithCost(2)
                    .WithStrategy(new MoveStrategy(_navMeshAgent, () => workPosition.position))
                    .WithEffect(Beliefs[Consts.Beliefs.AGENT_AT_WORK])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.WORK_FOR_GOLD)
                    .WithCost(6)
                    .WithStrategy(new EarnGoldStrategy(
                        this,
                        5f,
                        () => Mathf.RoundToInt(workGoldPerTick * (HasPickaxe ? pickaxeGoldMultiplier : 1f)),
                        () => workStaminaCost * (HasPickaxe ? pickaxeStaminaMultiplier : 1f)
                    ))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_WORK])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_SWORD])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_PICKAXE])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_COFFEE])
                    .Build());
            }

            if (minePosition != null)
            {
                actions.Add(new AgentAction.Builder(Consts.Actions.GO_MINE)
                    .WithCost(3)
                    .WithStrategy(new MoveStrategy(_navMeshAgent, () => minePosition.position))
                    .WithEffect(Beliefs[Consts.Beliefs.AGENT_AT_MINE])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.MINE_FOR_GOLD)
                    .WithCost(7)
                    .WithStrategy(new EarnGoldStrategy(
                        this,
                        5f,
                        () => Mathf.RoundToInt(mineGoldPerTick * (HasPickaxe ? pickaxeGoldMultiplier : 1f)),
                        () => mineStaminaCost * (HasPickaxe ? pickaxeStaminaMultiplier : 1f)
                    ))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_MINE])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_SWORD])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_PICKAXE])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_COFFEE])
                    .Build());
            }

            if (shopPosition != null)
            {
                actions.Add(new AgentAction.Builder(Consts.Actions.GO_TO_SHOP)
                    .WithCost(2)
                    .WithStrategy(new MoveStrategy(_navMeshAgent, () => shopPosition.position))
                    .WithEffect(Beliefs[Consts.Beliefs.AGENT_AT_SHOP])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.BUY_PICKAXE)
                    .WithCost(0.5f)
                    .WithStrategy(new BuyPickaxeStrategy(this, 1.2f, pickaxeCost))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_SHOP])
                    .WithPrecondition(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_PICKAXE])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_PICKAXE])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.BUY_COFFEE)
                    .WithCost(0.5f)
                    .WithStrategy(new BuyCoffeeStrategy(this, 0.8f, coffeeCost))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_SHOP])
                    .WithPrecondition(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_COFFEE])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_COFFEE])
                    .Build());

                actions.Add(new AgentAction.Builder(Consts.Actions.BUY_SWORD)
                    .WithCost(1)
                    .WithStrategy(new BuySwordStrategy(this, 1.5f, swordCost))
                    .WithPrecondition(Beliefs[Consts.Beliefs.AGENT_AT_SHOP])
                    .WithPrecondition(Beliefs[Consts.Beliefs.HAS_ENOUGH_GOLD_FOR_SWORD])
                    .WithEffect(Beliefs[Consts.Beliefs.HAS_SWORD])
                    .Build());
            }

            actions.Add(new AgentAction.Builder(Consts.Actions.DRINK_COFFEE)
                .WithCost(1)
                .WithStrategy(new DrinkCoffeeStrategy(this, 0.4f, coffeeStaminaGain, coffeeMaxCap))
                .WithPrecondition(Beliefs[Consts.Beliefs.HAS_COFFEE])
                .WithEffect(Beliefs[Consts.Beliefs.AGENT_STAMINA_OK])
                .Build());
        }

        private void SetupGoals()
        {
            goals = new HashSet<AgentGoal>();

            goals.Add(new AgentGoal.Builder(Consts.Goals.GET_RESTED)
                .WithPriority(6)
                .WithDesiredEffect(Beliefs[Consts.Beliefs.AGENT_IS_RESTED])
                .Build());

            goals.Add(new AgentGoal.Builder(Consts.Goals.GET_STAMINA_OK)
                .WithPriority(5)
                .WithDesiredEffect(Beliefs[Consts.Beliefs.AGENT_STAMINA_OK])
                .Build());

            goals.Add(new AgentGoal.Builder(Consts.Goals.HAVE_SWORD)
                .WithPriority(4)
                .WithDesiredEffect(Beliefs[Consts.Beliefs.HAS_SWORD])
                .Build());
            
            goals.Add(new AgentGoal.Builder(Consts.Goals.WANDER)
                .WithPriority(1)
                .WithDesiredEffect(Beliefs[Consts.Beliefs.AGENT_MOVING])
                .Build());

            goals.Add(new AgentGoal.Builder(Consts.Goals.CHILL_OUT)
                .WithPriority(0)
                .WithDesiredEffect(Beliefs[Consts.Beliefs.NOTHING])
                .Build());
        }

        private void Update()
        {
            _statsTimer?.Tick(Time.deltaTime);
            _animations?.SetSpeed(_navMeshAgent.velocity.magnitude);

            if (Beliefs != null && Beliefs.TryGetValue(Consts.Beliefs.AGENT_IS_TIRED, out var tired) && tired.Evaluate())
            {
                var isRestingFlow =
                    CurrentAction != null &&
                    (CurrentAction.Name == Consts.Actions.GO_HOME || CurrentAction.Name == Consts.Actions.REST_AT_HOME);

                if (!isRestingFlow)
                {
                    CurrentGoal = null;
                    CurrentAction = null;
                    ActionPlan = null;
                }
            }

            if (CurrentAction == null)
            {
                CalculatePlan();

                if (ActionPlan != null && ActionPlan.Actions.Count > 0)
                {
                    _navMeshAgent.ResetPath();
                    CurrentGoal = ActionPlan.AgentGoal;
                    CurrentAction = ActionPlan.Actions.Pop();
                    CurrentAction.Start();
                }
            }

            if (ActionPlan != null && CurrentAction != null)
            {
                CurrentAction.Update(Time.deltaTime);

                if (CurrentAction.Complete)
                {
                    CurrentAction.Stop();
                    CurrentAction = null;

                    if (ActionPlan.Actions.Count == 0)
                    {
                        lastGoal = CurrentGoal;
                        CurrentGoal = null;
                    }
                }
            }
        }

        private void CalculatePlan()
        {
            var priorityLevel = CurrentGoal?.Priority ?? 0;
            var goalsToCheck = goals;

            if (CurrentGoal != null)
            {
                goalsToCheck = new HashSet<AgentGoal>(goals.Where(goal => goal.Priority > priorityLevel));
            }

            if (HasSword)
            {
                goalsToCheck = new HashSet<AgentGoal>(goalsToCheck.Where(g => g.Name != Consts.Goals.HAVE_SWORD));
            }

            var newPlan = _planner.Plan(this, goalsToCheck, lastGoal);
            if (newPlan != null)
            {
                ActionPlan = newPlan;
            }
        }
    }
}
