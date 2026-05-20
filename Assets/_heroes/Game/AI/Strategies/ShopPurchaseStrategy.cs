using Heroes.Content.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Buildings;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.AI.Strategies
{
    public sealed class ShopPurchaseStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _context;
        private readonly BuildingFacade _building;
        private readonly ItemDefinition _item;
        private Vector3 _destination;

        private bool _purchased;

        public bool CanPerform => !_purchased;

        public bool Complete { get; private set; }

        public ShopPurchaseStrategy(
            Agent<GameWorldSnapshot, HeroAnimationController> agent,
            AgentContext<GameWorldSnapshot> context,
            BuildingFacade building,
            ItemDefinition item)
        {
            _agent = agent;
            _context = context;
            _building = building;
            _item = item;
            _destination = building != null ? building.DoorWorldPosition : agent.transform.position;
        }

        public void Start()
        {
            Complete = false;
            _purchased = false;

            if (_agent?.NavAgent != null)
            {
                if (NavMesh.SamplePosition(_destination, out var hit, 4f, NavMesh.AllAreas))
                {
                    _destination = hit.position;
                }
                _agent.NavAgent.SetDestination(_destination);
            }
        }

        public void Update(float deltaTime)
        {
            if (_agent == null || _context == null)
            {
                Complete = true;
                return;
            }

            
            _context.MutateState((ref AgentState s) => s.SetLocation(_agent.transform.position));

            if (_purchased)
            {
                Complete = true;
                return;
            }

            if (_agent.NavAgent == null)
            {
                Complete = true;
                return;
            }

            if (_agent.NavAgent.pathPending)
            {
                return;
            }

            
            if (_building != null)
            {
                var dist = Vector3.Distance(_agent.transform.position, _destination);
                if (dist > 1.0f)
                {
                    return;
                }
            }
            else if (_agent.NavAgent.remainingDistance > 2f)
            {
                return;
            }

            TryPurchase();
            Complete = true;
        }

        public void Stop()
        {
            if (_agent?.NavAgent != null)
            {
                _agent.NavAgent.ResetPath();
            }

            _context?.MutateState((ref AgentState s) => s.SetLocation(_agent.transform.position));
        }

        private void TryPurchase()
        {
            if (_purchased || _building?.Model == null || _building.Definition == null || _item == null)
            {
                return;
            }

            
            if (!_building.Model.IsSellItemUnlocked(_item.Id))
            {
                return;
            }

            var heroFacade = _agent.GetComponent<global::Heroes.Game.Heroes.HeroFacade>();
            if (heroFacade?.Model == null)
            {
                return;
            }

            var cost = _item.GoldCost;
            if (cost <= 0 || heroFacade.Model.Gold < cost)
            {
                return;
            }

            if (!heroFacade.Model.TryAddAndAutoEquip(_item))
            {
                return;
            }

            heroFacade.ApplyEquippedItemVisual(_item);
            heroFacade.ApplyItemEffects(_item, ItemEffectTrigger.Equip);

            heroFacade.Model.SetGold(heroFacade.Model.Gold - cost);
            _purchased = true;
        }
    }
}


