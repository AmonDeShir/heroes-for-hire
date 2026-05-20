using UnityEngine.UIElements;

namespace Heroes.GOAP.Editor
{
    public interface IGoapWorldDebugRenderer
    {
        bool CanRender(object snapshot);
        void Render(object snapshot, VisualElement root);
    }
}


