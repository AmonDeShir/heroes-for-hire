using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.Systems.GOAP.Demo;
using UnityEngine;

namespace GOAP.Demo
{
    public class DemoHeroAgent : Agent<DemoWorldSnapshot>
    {
        [SerializeField]
        private DemoWorldStateManager worldStateManager;
        
        protected override Archetype<Agent<DemoWorldSnapshot>, DemoWorldSnapshot> CreateArchetype()
        {
            var snapshot = worldStateManager.State.CreateSnapshot();
            var home = snapshot.Locations[DemoConsts.HOME];
            return new DemoArchetype(home);
        }

        protected override IWorldState<DemoWorldSnapshot> CreateWorldState()
        {
            return worldStateManager.State;
        }
    }
}
