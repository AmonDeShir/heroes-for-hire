using System.Collections.Generic;
using System.Linq;
using Registry;

namespace Heroes.Game.Heroes
{
    public sealed class HeroRosterService
    {
        public IReadOnlyList<HeroFacade> GetAll()
        {
            return Registry<HeroFacade>.All().Where(item => item != null).ToArray();
        }

        public bool TryGetById(string id, out HeroFacade hero)
        {
            hero = Registry<HeroFacade>.Get(items => items.FirstOrDefault(item => item != null && item.Id == id));
            return hero != null;
        }
    }
}


