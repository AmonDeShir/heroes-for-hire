using Heroes.Game.Combat;
using Heroes.Game.Monsters;
using UnityEngine;

namespace Heroes.Game.Heroes
{
    public sealed class HeroEnemySensor : ProximitySensor
    {
        protected override void Awake()
        {
            enemyTeam = TeamType.Enemies;
            base.Awake();
        }

        protected override Transform GetEnemyRoot(Collider other)
        {
            var m = other != null ? other.GetComponentInParent<MonsterFacade>() : null;
            return m != null && m.IsAlive ? m.transform : null;
        }

        protected override bool IsValidEnemy(Transform enemy)
        {
            var m = enemy != null ? enemy.GetComponent<MonsterFacade>() : null;
            return m != null && m.IsAlive;
        }
    }
}
