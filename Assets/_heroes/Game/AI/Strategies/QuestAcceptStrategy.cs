using Heroes.Game.Quests;
using Heroes.Game.Heroes;
using Heroes.GOAP;
using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public sealed class QuestAcceptStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly QuestService _quests;
        private bool _done;

        public bool CanPerform => !_done;
        public bool Complete { get; private set; }

        public QuestAcceptStrategy(Agent<GameWorldSnapshot, HeroAnimationController> agent, QuestService quests)
        {
            _agent = agent;
            _quests = quests;
        }

        public void Start()
        {
            Complete = false;
            _done = false;
        }

        public void Update(float deltaTime)
        {
            if (Complete || _done || _agent == null || _quests == null)
            {
                Complete = true;
                return;
            }

            var hero = _agent.GetComponent<HeroFacade>();
            if (hero?.Model == null)
            {
                Complete = true;
                return;
            }

            var best = _quests.GetBestQuestForHero(hero);
            if (!best.Exists)
            {
                Complete = true;
                return;
            }

            if (_quests.TryAccept(best.QuestId, hero.Model.InstanceId))
            {
                hero.Model.SetActiveQuest(best.QuestId, best.TargetInstanceId, best.TargetKind);
            }

            _done = true;
            Complete = true;
        }

        public void Stop()
        {
        }
    }
}
