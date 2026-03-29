using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IBuildingCatalog
    {
        IReadOnlyList<IBuildingDefinition> GetAll();
        IBuildingDefinition GetById(string id);
    }
}
