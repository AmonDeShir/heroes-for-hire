using System.Collections.Generic;
using System.Linq;
using Heroes.Game.Abstractions;
using UnityEngine;

namespace Heroes.Content.Definitions.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Catalog")]
    public class BuildingCatalogAsset : ScriptableObject, IBuildingCatalog
    {
        [SerializeField] private List<BuildingDefinition> buildings = new();

        public IReadOnlyList<IBuildingDefinition> GetAll() => buildings;

        public IBuildingDefinition GetById(string id)
        {
            return buildings.FirstOrDefault(x => x.Id == id);
        }
    }
}