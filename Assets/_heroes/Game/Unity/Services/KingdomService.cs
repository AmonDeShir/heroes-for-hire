using EventBus;
using Heroes.Game.Core;
using Heroes.Game.Core.Events;

namespace Heroes.Game.Buildings
{
    public sealed class KingdomService
    {
        private readonly KingdomModel _kingdom;

        public KingdomService(KingdomModel kingdom)
        {
            _kingdom = kingdom;
        }

        public int Gold => _kingdom.Gold;
        public int Population => _kingdom.Population;

        public bool CanAfford(int amount)
        {
            return _kingdom.CanAfford(amount);
        }

        public bool TrySpendGold(int amount)
        {
            if (!_kingdom.TrySpendGold(amount))
            {
                return false;
            }

            EventBus<GoldChangedEvent>.Invoke(new GoldChangedEvent { Value = _kingdom.Gold });
            return true;
        }

        public void AddGold(int amount)
        {
            _kingdom.AddGold(amount);
            EventBus<GoldChangedEvent>.Invoke(new GoldChangedEvent { Value = _kingdom.Gold });
        }

        public void AddPeople(int amount)
        {
            _kingdom.AddPeople(amount);
            EventBus<PopulationChangedEvent>.Invoke(new PopulationChangedEvent { Value = _kingdom.Population });
        }

        public void RemovePeople(int amount)
        {
            _kingdom.RemovePeople(amount);
            EventBus<PopulationChangedEvent>.Invoke(new PopulationChangedEvent { Value = _kingdom.Population });
        }
    }
}
