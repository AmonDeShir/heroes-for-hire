using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace Heroes.GOAP.Editor
{
    public static class GoapWorldDebugRendererRegistry
    {
        private static readonly List<IGoapWorldDebugRenderer> Renderers = new List<IGoapWorldDebugRenderer>();
        private static readonly IGoapWorldDebugRenderer Fallback = new ReflectionWorldDebugRenderer();

        public static void Register(IGoapWorldDebugRenderer renderer)
        {
            if (renderer == null || Renderers.Contains(renderer))
            {
                return;
            }

            Renderers.Add(renderer);
        }

        public static void Unregister(IGoapWorldDebugRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            Renderers.Remove(renderer);
        }

        public static IGoapWorldDebugRenderer Resolve(object snapshot)
        {
            if (snapshot == null)
            {
                return Fallback;
            }

            foreach (var renderer in Renderers)
            {
                if (renderer.CanRender(snapshot))
                {
                    return renderer;
                }
            }

            return Fallback;
        }

        private sealed class ReflectionWorldDebugRenderer : IGoapWorldDebugRenderer
        {
            public bool CanRender(object snapshot) => true;

            public void Render(object snapshot, VisualElement root)
            {
                root.Clear();

                if (snapshot == null)
                {
                    root.Add(new Label("No world snapshot."));
                    return;
                }

                var type = snapshot.GetType();
                root.Add(new Label(type.Name));

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(prop => prop.CanRead && prop.GetIndexParameters().Length == 0)
                    .Where(prop => !IsReservedName(prop.Name))
                    .OrderBy(prop => prop.Name, StringComparer.Ordinal);

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(snapshot, null);
                    root.Add(new Label($"{prop.Name}: {FormatValue(value)}"));
                }

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => !IsReservedName(field.Name))
                    .OrderBy(field => field.Name, StringComparer.Ordinal);

                foreach (var field in fields)
                {
                    var value = field.GetValue(snapshot);
                    root.Add(new Label($"{field.Name}: {FormatValue(value)}"));
                }
            }

            private static bool IsReservedName(string name)
            {
                return name == "Version" || name == "IsValid";
            }

            private static string FormatValue(object value)
            {
                return value == null ? "<null>" : value.ToString();
            }
        }
    }
}


