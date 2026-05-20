using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Content.Heroes
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Catalog")]
    public class ItemCatalog : ScriptableObject
    {
        [SerializeField] private ItemDefinition[] items;

        private Dictionary<string, ItemDefinition> _byId;

        public IReadOnlyList<ItemDefinition> GetAll()
        {
            return items;
        }

        public ItemDefinition GetById(string id)
        {
            if (_byId == null)
            {
                Initialize();
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return _byId != null && _byId.TryGetValue(id, out var def) ? def : null;
        }

        public void Initialize()
        {
            _byId = new Dictionary<string, ItemDefinition>();

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id))
                {
                    continue;
                }

                _byId[item.Id] = item;
            }
        }
    }
}


