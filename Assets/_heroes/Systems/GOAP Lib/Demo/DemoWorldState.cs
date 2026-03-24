using System.Collections.Generic;
using Heroes.GOAP.Core;
using UnityEngine;

namespace GOAP.Demo
{
    public sealed class DemoWorldState : WorldStateBase<DemoWorldSnapshot>
    {
        private Dictionary<string, Vector2> Locations;
        
        public DemoWorldState()
        {
            Locations = new Dictionary<string, Vector2>();
        }
        
        public override DemoWorldSnapshot CreateSnapshot()
        {
            return new DemoWorldSnapshot(Version, true, Locations);
        }

        public void RegisterLocation(string name, Vector2 location)
        {
            Locations[name] = location;
        }
    }
}
