using Unity.GraphToolkit.Editor;

namespace Heroes.Goap.Editor.Utilities
{
    internal static class GoapNodeOptionReader
    {
        public static T GetOption<T>(Node node, string name, T defaultValue)
        {
            var option = node.GetNodeOptionByName(name);
            if (option != null && option.TryGetValue(out T value))
                return value;

            return defaultValue;
        }
    }
}
