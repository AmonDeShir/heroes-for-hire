using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Heroes.Game.AI;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Game.Heroes;
using Registry;
using UnityEngine;
using VContainer;

namespace Heroes.Game.Runtime
{
    public sealed class HeroReviveService : MonoBehaviour
    {
        [SerializeField] private float reviveDelaySeconds = 60f;
        [SerializeField] private float reviveHpPct = 0.05f;

        private HeroRosterService _roster;
        private EventBinding<HealthChangedEvent> _healthChanged;
        private readonly HashSet<string> _pending = new();
        private readonly Dictionary<string, float> _reviveAt = new();

        [Inject]
        public void Construct(HeroRosterService roster)
        {
            _roster = roster;
        }

        private void Awake()
        {
            _healthChanged = new EventBinding<HealthChangedEvent>(OnHealthChanged);
            EventBus<HealthChangedEvent>.Register(_healthChanged);
        }

        private void OnDestroy()
        {
            EventBus<HealthChangedEvent>.Unregister(_healthChanged);
        }

        private void OnHealthChanged(HealthChangedEvent e)
        {
            if (string.IsNullOrWhiteSpace(e.Id) || _roster == null)
            {
                return;
            }

            if (e.Value > 0f)
            {
                _pending.Remove(e.Id);
                _reviveAt.Remove(e.Id);
                return;
            }

            if (!_roster.TryGetById(e.Id, out var hero) || hero == null || hero.Model == null)
            {
                return;
            }

            if (hero.Model.IsAlive)
            {
                return;
            }

            if (_pending.Contains(e.Id))
            {
                return;
            }

            if (!TryGetChapel(out var chapel))
            {
                return;
            }

            _pending.Add(e.Id);
            _reviveAt[e.Id] = Time.unscaledTime + Mathf.Max(0.1f, reviveDelaySeconds);
            StartCoroutine(ReviveAfterDelay(e.Id));
        }

        private IEnumerator ReviveAfterDelay(string heroId)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, reviveDelaySeconds));

            _pending.Remove(heroId);
            _reviveAt.Remove(heroId);

            if (_roster == null || !_roster.TryGetById(heroId, out var hero) || hero == null || hero.Model == null)
            {
                yield break;
            }

            if (hero.Model.IsAlive)
            {
                yield break;
            }

            if (!TryGetChapel(out var chapel) || chapel == null)
            {
                yield break;
            }

            hero.ReviveAt(chapel.DoorWorldPosition, reviveHpPct);
        }

        public bool TryGetPending(string heroId, out float remainingSeconds, out float totalSeconds)
        {
            remainingSeconds = 0f;
            totalSeconds = Mathf.Max(0.1f, reviveDelaySeconds);

            if (string.IsNullOrWhiteSpace(heroId) || !_pending.Contains(heroId))
            {
                return false;
            }

            if (!_reviveAt.TryGetValue(heroId, out var at))
            {
                return false;
            }

            remainingSeconds = Mathf.Max(0f, at - Time.unscaledTime);
            return true;
        }

        public IReadOnlyCollection<string> PendingHeroIds => _pending;

        internal static bool TryGetChapel(out BuildingFacade chapel)
        {
            chapel = null;
            var defId = GoapRuntimeConfig.Buildings != null && GoapRuntimeConfig.Buildings.Chapel != null
                ? GoapRuntimeConfig.Buildings.Chapel.Id
                : string.Empty;

            if (string.IsNullOrWhiteSpace(defId))
            {
                return false;
            }

            chapel = Registry<BuildingFacade>.All()
                .Where(b => b != null && b.IsAlive && b.Definition != null && b.Definition.Id == defId)
                .OrderBy(b => b.transform.position.sqrMagnitude)
                .FirstOrDefault();

            return chapel != null;
        }
    }
}
