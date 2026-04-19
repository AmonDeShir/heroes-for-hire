using Heroes.Presentation.UI.BuildingPanel;
using UnityEngine;

namespace Heroes.Game.Abstractions
{
    public interface ISelectable
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }
    }
}