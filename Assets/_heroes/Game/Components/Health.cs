using Unity.Entities;

namespace Heroes.Game.Components
{
    public struct Health : IComponentData
    {
        public float max;
        public float value;
        public float regen;
    }
}