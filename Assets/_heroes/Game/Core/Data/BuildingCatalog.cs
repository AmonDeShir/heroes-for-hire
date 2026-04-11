using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Content.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Catalog")]
    public class BuildingCatalog : ScriptableObject
    {
        [SerializeField] private BuildingDefinition[] buildings;

        private Dictionary<string, BuildingDefinition> _byId;

        public IReadOnlyList<BuildingDefinition> GetAll()
        {
            return buildings;
        }

        public BuildingDefinition GetById(string id)
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
            _byId = new Dictionary<string, BuildingDefinition>();

            if (buildings == null)
            {
                return;
            }

            foreach (var building in buildings)
            {
                if (building == null || string.IsNullOrWhiteSpace(building.Id))
                {
                    continue;
                }

                _byId[building.Id] = building;
            }
        }
    }
}
