using System.Collections.Generic;
using System.Linq;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;

namespace Heroes.Presentation.UI.BuildMenu
{
    public class BuildMenuViewModel
    {
        private readonly IBuildingCatalog _catalog;
        private readonly IBuildingPlacementSelectionService _selection;
        private readonly IKingdomResources _resources;
        private readonly IGameEventBus _eventBus;

        public BuildMenuViewModel(
            IBuildingCatalog catalog,
            IBuildingPlacementSelectionService selection,
            IKingdomResources resources,
            IGameEventBus eventBus)
        {
            _catalog = catalog;
            _selection = selection;
            _resources = resources;
            _eventBus = eventBus;
        }

        public IReadOnlyList<BuildMenuItemVm> GetItems()
        {
            return _catalog.GetAll()
                .Select(x => new BuildMenuItemVm(
                    x.Id,
                    x.DisplayName,
                    x.GoldCost,
                    _resources.Gold >= x.GoldCost,
                    _selection.SelectedBuildingDefinitionId == x.Id))
                .ToList();
        }

        public void SelectBuilding(string id)
        {
            _selection.Select(id);
        }

        public int GetGold() => _resources.Gold;
    }

    public readonly struct BuildMenuItemVm
    {
        public string Id { get; }
        public string Name { get; }
        public int Cost { get; }
        public bool Available { get; }
        public bool Selected { get; }

        public BuildMenuItemVm(string id, string name, int cost, bool available, bool selected)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Available = available;
            Selected = selected;
        }
    }
}
