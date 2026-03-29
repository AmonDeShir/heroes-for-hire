using System.Collections.Generic;

namespace Heroes.Content.Definitions.Common
{
    public static class DefinitionListUtility
    {
        public static IReadOnlyList<TInterface> ToInterfaceList<TStruct, TInterface>(List<TStruct> source)
            where TStruct : TInterface
        {
            if (source == null || source.Count == 0)
            {
                return System.Array.Empty<TInterface>();
            }

            var list = new List<TInterface>(source.Count);
            for (var i = 0; i < source.Count; i++)
            {
                list.Add(source[i]);
            }

            return list;
        }
    }
}
