using Heroes.Content.Heroes;
using Heroes.Game.Combat;
using UnityEngine;
using VContainer;

namespace Heroes.Game.Runtime
{
    public sealed class CombatBootstrap : MonoBehaviour
    {
        public CombatService Service { get; private set; }

        [Inject]
        public void Construct(ItemCatalog items)
        {
            Service = new CombatService(items);
            CombatRuntimeConfig.Set(Service);
        }
    }
}
