using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using Heroes.Systems.GOAP.Demo;
using UnityEngine;
using WebLess;

namespace GOAP.Demo
{
    public class DemoHeroAgent : Agent<DemoWorldSnapshot, DemoCharacterAnimationController>, IBeliefNameProvider
    {
        [SerializeField]
        private DemoWorldStateManager worldStateManager;
        
        protected override Archetype<Agent<DemoWorldSnapshot, DemoCharacterAnimationController>, DemoWorldSnapshot> CreateArchetype()
        {
            var snapshot = worldStateManager.State.CreateSnapshot();
            var home = snapshot.Locations[DemoConsts.HOME];
            return new DemoArchetype(home);
        }

        protected override IWorldState<DemoWorldSnapshot> CreateWorldState()
        {
            return worldStateManager.State;
        }

        public bool TryGetBeliefName(int index, out string name)
        {
            name = index switch
            {
                DemoConsts.GOLD => "Gold",
                DemoConsts.PICKAXE => "Pickaxe",
                DemoConsts.SWORD => "Sword",
                _ => string.Empty
            };

            return !string.IsNullOrEmpty(name);
        }
    }
}


