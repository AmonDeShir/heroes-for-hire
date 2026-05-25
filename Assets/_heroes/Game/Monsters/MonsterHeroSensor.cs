using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Game.Monsters
{
    public sealed class MonsterHeroSensor : ProximitySensor
    {
        protected override void Awake()
        {
            enemyTeam = TeamType.Heroes;
            base.Awake();
        }

        protected override Transform GetEnemyRoot(Collider other)
        {
            var h = other != null ? other.GetComponentInParent<HeroFacade>() : null;
            return h != null && h.IsAlive ? h.transform : null;
        }

        protected override bool IsValidEnemy(Transform enemy)
        {
            var h = enemy != null ? enemy.GetComponent<HeroFacade>() : null;
            return h != null && h.IsAlive;
        }
    }
}
