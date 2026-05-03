using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class HeroAgent : Agent<GameWorldSnapshot, HeroAnimationController>, IBeliefNameProvider
    {
        private HeroFacade _hero;
        private GameWorldStateManager _worldStateManager;

        public void Initialize(HeroFacade hero, GameWorldStateManager worldStateManager)
        {
            _hero = hero;
            _worldStateManager = worldStateManager;
        }

        protected override Archetype<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> CreateArchetype()
        {
            return new HeroArchetype(_hero);
        }

        protected override IWorldState<GameWorldSnapshot> CreateWorldState()
        {
            return _worldStateManager != null ? _worldStateManager.State : new GameWorldState();
        }

        public bool TryGetBeliefName(int index, out string name)
        {
            name = index switch
            {
                Consts.GOLD => "Gold",
                Consts.HEALTH => "Health",
                Consts.GEAR_LEVEL => "Gear Level",
                Consts.DANGER_LEVEL => "Danger",
                _ => string.Empty,
            };

            return !string.IsNullOrWhiteSpace(name);
        }

        public new void Update()
        {
            SyncStateFromModel();
            base.Update();
        }

        public bool IsInsideHome()
        {
            if (_hero?.Model == null || _worldStateManager?.State == null)
            {
                return false;
            }

            var snapshot = _worldStateManager.State.CreateSnapshot();
            if (!snapshot.Locations.TryGetPositionByInstanceId(_hero.Model.HomeBuildingInstanceId, out var homePosition))
            {
                return false;
            }

            var currentPosition = new Vector2(transform.position.x, transform.position.z);
            return Vector2.Distance(currentPosition, homePosition) <= _hero.Model.HomeRadius;
        }

        private void SyncStateFromModel()
        {
            if (_hero?.Model == null || PlanExecutor?.Context == null)
            {
                return;
            }

            PlanExecutor.Context.MutateState((ref AgentState state) =>
            {
                state.SetLocation(transform.position);
                state.SetBelieve(Consts.GOLD, _hero.Model.Gold);
                state.SetBelieve(Consts.HEALTH, _hero.Model.Health.Current);
                state.SetBelieve(Consts.GEAR_LEVEL, _hero.Model.GearLevel);
                state.SetBelieve(Consts.DANGER_LEVEL, _hero.Model.DangerLevel);
            });

            executor.CalculatePlan();
        }
    }
}
