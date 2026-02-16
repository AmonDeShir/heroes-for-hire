using System;
using System.Reflection;
using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Utilities
{
    internal static class GoapPortCapacityHelper
    {
        public static void SetMulti(IPort port)
        {
            if (port == null)
                return;

            var property = port.GetType().GetProperty("Capacity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
                return;

            var enumType = property.PropertyType;
            if (!enumType.IsEnum)
                return;

            var multi = Enum.Parse(enumType, "Multi");
            property.SetValue(port, multi);
        }

        public static void SetNoEmbeddedConstant(IPort port)
        {
            if (port == null)
                return;

            var property = port.GetType().GetProperty("Options", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null)
                return;

            var enumType = property.PropertyType;
            if (!enumType.IsEnum)
                return;

            var current = property.GetValue(port);
            var noEmbedded = Enum.Parse(enumType, "NoEmbeddedConstant");
            var combined = Convert.ToInt64(current) | Convert.ToInt64(noEmbedded);
            var value = Enum.ToObject(enumType, combined);
            property.SetValue(port, value);

            TryRemoveEmbeddedConstant(port);
        }

        static void TryRemoveEmbeddedConstant(IPort port)
        {
            var nodeModelProperty = port.GetType().GetProperty("NodeModel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var uniqueNameProperty = port.GetType().GetProperty("UniqueName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (nodeModelProperty == null || uniqueNameProperty == null)
                return;

            var nodeModel = nodeModelProperty.GetValue(port);
            var uniqueName = uniqueNameProperty.GetValue(port) as string;
            if (nodeModel == null || string.IsNullOrWhiteSpace(uniqueName))
                return;

            var field = nodeModel.GetType().GetField("m_InputConstantsById", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                return;

            var dictionary = field.GetValue(nodeModel);
            if (dictionary == null)
                return;

            var removeMethod = dictionary.GetType().GetMethod("Remove", new[] { typeof(string) });
            removeMethod?.Invoke(dictionary, new object[] { uniqueName });
        }
    }
}
