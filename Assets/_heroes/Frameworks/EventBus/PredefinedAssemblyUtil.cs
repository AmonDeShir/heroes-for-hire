using System;
using System.Collections.Generic;
using System.Reflection;

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
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
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
                    assemblyTypes.Add((AssemblyType)assemblyType, SafeGetTypes(assembly));
                }
            }

            if (assemblyTypes.Count == 0)
            {
                foreach (var assembly in assemblies)
                {
                    AddTypesFromAssembly(SafeGetTypes(assembly), interfaceType, types);
                }

                return types;
            }

            AddIfPresent(assemblyTypes, AssemblyType.AssemblyCSharp, interfaceType, types);
            AddIfPresent(assemblyTypes, AssemblyType.AssemblyCSharpEditor, interfaceType, types);
            AddIfPresent(assemblyTypes, AssemblyType.AssemblyCSharpFirstPass, interfaceType, types);
            AddIfPresent(assemblyTypes, AssemblyType.AssemblyCSharpEditorFirstPass, interfaceType, types);

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

        private static void AddIfPresent(Dictionary<AssemblyType, Type[]> assemblyTypes, AssemblyType key, Type interfaceType, ICollection<Type> types)
        {
            if (assemblyTypes.TryGetValue(key, out var assembly))
            {
                AddTypesFromAssembly(assembly, interfaceType, types);
            }
        }

        private static Type[] SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types ?? Array.Empty<Type>();
            }
        }
    }
}
