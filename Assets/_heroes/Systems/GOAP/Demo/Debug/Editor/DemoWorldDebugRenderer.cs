using Heroes.GOAP.Editor;
using UnityEngine.UIElements;

namespace GOAP.Demo.Debug
{
    public sealed class DemoWorldDebugRenderer : IGoapWorldDebugRenderer
    {
        public bool CanRender(object snapshot)
        {
            return snapshot is DemoWorldSnapshot;
        }

        public void Render(object snapshot, VisualElement root)
        {
            root.Clear();

            if (snapshot is not DemoWorldSnapshot demoSnapshot)
            {
                root.Add(new Label("No demo world snapshot."));
                return;
            }

            root.Add(new Label($"Locations: {demoSnapshot.Locations.Count}"));
            
            foreach (var kvp in demoSnapshot.Locations)
            {
                root.Add(new Label($"{kvp.Key} -> {kvp.Value}"));
            }
        }
    }
}
