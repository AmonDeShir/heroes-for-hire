using EventBus;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;

namespace Heroes.Game.Buildings
{
    public class SelectionService
    {
        public ISelectable Selected { get; private set; }
        public string SelectedId => Selected?.Id ?? "";

        public void Select(ISelectable item)
        {
            if (item == null)
            {
                return;
            }

            Selected = item;
            EventBus<ObjectSelectedEvent>.Invoke(new ObjectSelectedEvent { value = item });
        }
        
        public void Clear()
        {
            Selected = null;
            EventBus<ObjectSelectedEvent>.Invoke(new ObjectSelectedEvent { value = null });
        }
    }
}