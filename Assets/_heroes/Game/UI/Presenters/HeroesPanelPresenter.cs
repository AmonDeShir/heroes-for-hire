using System.Collections.Generic;
using System.Linq;
using Heroes.Game.Heroes;
using Heroes.Game.Buildings;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.HeroesPanel
{
    public partial class HeroesPanelPresenter : MonoBehaviour
    {
        private HeroRosterService _roster;
        private SelectionService _selection;

        private float _nextRefreshAt;

        [EventfulProperty] private HeroListItemDTO[] _heroes = System.Array.Empty<HeroListItemDTO>();

        [Inject]
        public void Construct(HeroRosterService roster, SelectionService selection)
        {
            _roster = roster;
            _selection = selection;

            Refresh();
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextRefreshAt)
            {
                return;
            }

            _nextRefreshAt = Time.unscaledTime + 0.25f;
            Refresh();
        }

        public void SelectHero(string heroId)
        {
            if (string.IsNullOrWhiteSpace(heroId) || _roster == null || _selection == null)
            {
                return;
            }

            if (_roster.TryGetById(heroId, out var hero) && hero != null)
            {
                _selection.Select(hero);
            }
        }

        private void Refresh()
        {
            if (_roster == null)
            {
                Heroes = System.Array.Empty<HeroListItemDTO>();
                return;
            }

            var all = _roster.GetAll();
            if (all == null || all.Count == 0)
            {
                Heroes = System.Array.Empty<HeroListItemDTO>();
                return;
            }

            var list = new List<HeroListItemDTO>(all.Count);
            for (var i = 0; i < all.Count; i++)
            {
                var h = all[i];
                if (h == null || h.Model == null)
                {
                    continue;
                }

                list.Add(new HeroListItemDTO(h.Id, h.Name, h.Icon, h.Health, h.MaxHealth));
            }

            Heroes = list.OrderBy(x => x.Name).ToArray();
        }
    }
}
