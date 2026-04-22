using EventBus;
using Heroes.Game.Core;
using Heroes.Game.Core.Events;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.ResourcesPanel
{
    public partial class KingdomResourcesPresenter : MonoBehaviour
    {
        private EventBinding<GoldChangedEvent> _goldEvent;
        private EventBinding<PopulationChangedEvent> _populationEvent;
        
        [EventfulProperty] private int _gold;
        [EventfulProperty] private int _population;
        
        [Inject]
        public void Construct(KingdomModel kingdom)
        {
            Gold = kingdom.Gold;
            Population = kingdom.Population;
        }

        private void OnEnable() {    
            _goldEvent = new EventBinding<GoldChangedEvent>(UpdateGold);
            _populationEvent = new EventBinding<PopulationChangedEvent>(UpdatePopulation);
            
            EventBus<GoldChangedEvent>.Register(_goldEvent);
            EventBus<PopulationChangedEvent>.Register(_populationEvent);
        }
        
        private void OnDisable() {
            EventBus<GoldChangedEvent>.Unregister(_goldEvent);
            EventBus<PopulationChangedEvent>.Unregister(_populationEvent);
        }
        
        private void UpdateGold(GoldChangedEvent @event)
        {
            Gold = @event.Value;
        }
        
        private void UpdatePopulation(PopulationChangedEvent @event)
        {
            Population = @event.Value;
        }
    }
}
