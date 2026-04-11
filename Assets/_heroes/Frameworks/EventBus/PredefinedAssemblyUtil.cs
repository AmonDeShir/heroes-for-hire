using System;
using System.Collections.Generic;

namespace EventBus
{
    public class PredefinedAssemblyUtil
    {
        private enum  AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpFirstPass,
            AssemblyCSharpEditorFirstPass,
        }

        private static AssemblyType? GetAssemblyType(string name)
        {
            return name switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                _ => null
            };
        }

        public static List<Type> GetTypes(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var assemblyType = GetAssemblyType(assembly.GetName().Name);

                if (assemblyType != null)
                {
                    assemblyTypes.Add((AssemblyType)assemblyType, assembly.GetTypes());
                }
            }

            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], interfaceType, types);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpEditor], interfaceType, types);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], interfaceType, types);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpEditorFirstPass], interfaceType, types);

            return types;
        }

        private static void AddTypesFromAssembly(Type[] assembly, Type interfaceType, ICollection<Type> types)
        {
            if (assembly == null)
            {
                return;
            }

            foreach (var type in assembly)
            {
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
        }
    }
}