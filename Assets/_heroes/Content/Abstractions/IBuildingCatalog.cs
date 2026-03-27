using System.Collections.Generic;

namespace Heroes.Game.Abstractions
{
    public interface IBuildingCatalog
    {
        IReadOnlyList<IBuildingDefinition> GetAll();
        IBuildingDefinition GetById(string id);
    }
}