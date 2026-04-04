using Unity.Entities;

namespace Heroes.Game.Components
{
    public struct DestroyPhaseElement : IBufferElementData
    {
        public Entity Value;
    }
}