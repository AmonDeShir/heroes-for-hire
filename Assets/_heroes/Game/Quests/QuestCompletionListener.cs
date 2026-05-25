using EventBus;
using Heroes.Game.Core.Events;
using UnityEngine;
using VContainer;

namespace Heroes.Game.Quests
{
    public sealed class QuestCompletionListener : MonoBehaviour
    {
        private QuestService _quests;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyed;
        private EventBinding<MonsterKilledEvent> _monsterKilled;

        [Inject]
        public void Construct(QuestService quests)
        {
            _quests = quests;
            QuestRuntimeConfig.Set(quests);
        }

        private void Awake()
        {
            _buildingDestroyed = new EventBinding<BuildingDestroyedEvent>(e => _quests?.CompleteByTarget(e.InstanceId));
            _monsterKilled = new EventBinding<MonsterKilledEvent>(e => _quests?.CompleteByTarget(e.InstanceId));
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyed);
            EventBus<MonsterKilledEvent>.Register(_monsterKilled);
        }

        private void OnDestroy()
        {
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyed);
            EventBus<MonsterKilledEvent>.Unregister(_monsterKilled);
        }
    }
}
