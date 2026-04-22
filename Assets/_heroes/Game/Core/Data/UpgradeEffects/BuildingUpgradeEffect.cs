using Heroes.Game.Buildings;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    public abstract class BuildingUpgradeEffect : ScriptableObject
    {
        public abstract void ApplyEffect(BuildingModel model);
    }
}
