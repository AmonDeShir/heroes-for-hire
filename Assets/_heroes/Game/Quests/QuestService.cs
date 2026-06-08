using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Game.Heroes;
using Heroes.Game.Monsters;
using Registry;
using UnityEngine;

namespace Heroes.Game.Quests
{
    public sealed class QuestService
    {
        private readonly KingdomService _kingdom;

        private readonly Dictionary<string, QuestInstance> _byId = new();
        private readonly Dictionary<string, string> _questIdByTarget = new();

        public QuestService(KingdomService kingdom)
        {
            _kingdom = kingdom;
        }

        public bool TryGetByTarget(string targetInstanceId, out QuestInstance quest)
        {
            quest = null;
            
            if (string.IsNullOrWhiteSpace(targetInstanceId))
            {
                return false;
            }

            return _questIdByTarget.TryGetValue(targetInstanceId, out var qid) && _byId.TryGetValue(qid, out quest) && quest != null;
        }

        public bool TryGetById(string questId, out QuestInstance quest)
        {
            quest = null;
            return !string.IsNullOrWhiteSpace(questId) && _byId.TryGetValue(questId, out quest) && quest != null;
        }

        public bool TryCreateCombatQuestForTarget(string targetInstanceId, QuestTargetKind kind, int baseGold, out string questId)
        {
            questId = string.Empty;
            baseGold = Mathf.Max(0, baseGold);
            
            if (baseGold <= 0 || string.IsNullOrWhiteSpace(targetInstanceId) || _kingdom == null)
            {
                return false;
            }

            if (_questIdByTarget.ContainsKey(targetInstanceId))
            {
                return false;
            }

            if (!_kingdom.TrySpendGold(baseGold))
            {
                return false;
            }

            questId = Guid.NewGuid().ToString();
            var q = new QuestInstance(questId, QuestType.Combat, kind, targetInstanceId, baseGold, Time.unscaledTime);
           
            _byId[questId] = q;
            _questIdByTarget[targetInstanceId] = questId;
            
            EventBus<QuestCreatedEvent>.Invoke(new QuestCreatedEvent { Value = questId });
            return true;
        }

        public bool TryIncreaseOffer(string questId, int deltaGold)
        {
            deltaGold = Mathf.Max(0, deltaGold);
            
            if (deltaGold <= 0 || _kingdom == null)
            {
                return false;
            }

            if (!TryGetById(questId, out var q) || q.State != QuestState.Active)
            {
                return false;
            }

            if (!_kingdom.TrySpendGold(deltaGold))
            {
                return false;
            }

            q.PoolGold += deltaGold;
            
            EventBus<QuestUpdatedEvent>.Invoke(new QuestUpdatedEvent { Value = questId });
            return true;
        }

        public bool TryAccept(string questId, string heroInstanceId)
        {
            if (string.IsNullOrWhiteSpace(questId) || string.IsNullOrWhiteSpace(heroInstanceId))
            {
                return false;
            }

            if (!TryGetById(questId, out var q) || q.State != QuestState.Active)
            {
                return false;
            }

            if (!q.Participants.Add(heroInstanceId))
            {
                return false;
            }

            EventBus<QuestAcceptedEvent>.Invoke(new QuestAcceptedEvent { QuestId = questId, HeroId = heroInstanceId });
            EventBus<QuestUpdatedEvent>.Invoke(new QuestUpdatedEvent { Value = questId });
            
            return true;
        }

        public void CompleteByTarget(string targetInstanceId)
        {
            if (string.IsNullOrWhiteSpace(targetInstanceId))
            {
                return;
            }

            if (!TryGetByTarget(targetInstanceId, out var q) || q.State != QuestState.Active)
            {
                return;
            }

            q.State = QuestState.Completed;
            _questIdByTarget.Remove(targetInstanceId);

            var count = q.Participants.Count;
            if (count > 0 && q.PoolGold > 0)
            {
                var share = q.PoolGold / count;
                if (share > 0)
                {
                    foreach (var heroId in q.Participants)
                    {
                        var hero = Registry<HeroFacade>.Get(items => items.FirstOrDefault(h => h != null && h.Model != null && h.Model.InstanceId == heroId));
                        if (hero != null && hero.Model != null && hero.Model.IsAlive)
                        {
                            hero.AddGold(share);
                        }
                    }
                }
            }

            EventBus<QuestCompletedEvent>.Invoke(new QuestCompletedEvent { Value = q.QuestId });
        }

        public BestQuestSnapshot GetBestQuestForHero(HeroFacade hero)
        {
            if (hero?.Model == null || _byId.Count == 0)
            {
                return default;
            }

            QuestInstance best = null;
            float bestScore = 0f;

            foreach (var pair in _byId)
            {
                var q = pair.Value;
                if (q == null || q.State != QuestState.Active || q.Type != QuestType.Combat)
                {
                    continue;
                }

                if (!TryResolveTarget(q, out var hp, out var pos))
                {
                    continue;
                }

                var participants = q.Participants.Count;
                var share = participants >= 0 ? (float)q.PoolGold / (participants + 1) : q.PoolGold;

                var dps = EstimateHeroDps(hero);
                if (dps <= 0.01f)
                {
                    continue;
                }

                var seconds = hp / dps;
                var score = seconds > 0.01f ? share / seconds : share;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = q;
                }
            }

            if (best == null)
            {
                return default;
            }

            TryResolveTarget(best, out var hp2, out var pos2);
            var isParticipant = best.Participants.Contains(hero.Model.InstanceId);
            return new BestQuestSnapshot(true, best.QuestId, best.TargetKind, best.TargetInstanceId, best.PoolGold, best.Participants.Count, hp2, pos2, isParticipant);
        }

        private static float EstimateHeroDps(HeroFacade hero)
        {
            if (hero?.Model == null)
            {
                return 0f;
            }

            var atk = hero.Definition != null ? hero.Definition.Attack : 1f;
            var dmg = Mathf.Max(0.1f, atk + hero.Model.EquipmentAttack + hero.Model.TimedAttack);
            return dmg;
        }

        private static bool TryResolveTarget(QuestInstance q, out float hp, out Vector3 pos)
        {
            hp = 0f;
            pos = Vector3.zero;

            if (q == null)
            {
                return false;
            }

            if (q.TargetKind == QuestTargetKind.Building)
            {
                var b = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(x => x != null && x.Id == q.TargetInstanceId));
                if (b == null || !b.IsAlive)
                {
                    return false;
                }
                hp = b.Health;
                pos = b.transform.position;
                return true;
            }

            if (q.TargetKind == QuestTargetKind.Monster)
            {
                var m = Registry<MonsterFacade>.Get(items => items.FirstOrDefault(x => x != null && x.InstanceId == q.TargetInstanceId));
                if (m == null || !m.IsAlive)
                {
                    return false;
                }
                hp = m.Health;
                pos = m.transform.position;
                return true;
            }

            return false;
        }
    }
}
