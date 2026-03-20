using System;
using System.Reflection;
using Unity.GraphToolkit.Editor;

namespace Heroes.Systems.GOAPGraph.Editor.Nodes
{
    public static class GraphToolkitPortExtensions
    {
        public static IPort WithMultiCapacity(this IPort port)
        {
            if (port == null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            var type = port.GetType();

            var prop = type.GetProperty("Capacity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
           
            if (prop == null)
            {
                throw new MissingMemberException(type.FullName, "Capacity");
            }

            var enumType = prop.PropertyType;
            
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException($"Capacity is not an enum. Type: {enumType.FullName}");
            }

            var multiValue = Enum.Parse(enumType, "Multi", ignoreCase: true);
            prop.SetValue(port, multiValue);

            return port;
        }
    }
}
