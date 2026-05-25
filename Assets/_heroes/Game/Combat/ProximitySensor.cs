using System.Collections.Generic;
using Heroes.Game.Combat;
using UnityEngine;

namespace Heroes.Game.Combat
{
    public abstract class ProximitySensor : MonoBehaviour
    {
        [SerializeField] protected SphereCollider trigger;
        [SerializeField] protected TeamType enemyTeam;

        protected readonly HashSet<Transform> enemies = new();

        public int EnemyCount => enemies.Count;

        public int GetEnemyCount()
        {
            enemies.RemoveWhere(t => t == null || !IsValidEnemy(t));
            return enemies.Count;
        }

        protected virtual void Awake()
        {
            if (trigger == null)
            {
                trigger = GetComponent<SphereCollider>();
            }

            if (trigger != null)
            {
                trigger.isTrigger = true;
            }
        }

        public bool TryGetNearestEnemy(Vector3 from, out Transform enemy)
        {
            enemies.RemoveWhere(t => t == null || !IsValidEnemy(t));

            enemy = null;
            var best = float.MaxValue;

            foreach (var t in enemies)
            {
                if (t == null || !IsValidEnemy(t))
                {
                    continue;
                }

                var d = (t.position - from).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    enemy = t;
                }
            }

            return enemy != null;
        }

        protected bool IsEnemy(Collider other)
        {
            if (other == null)
            {
                return false;
            }

            if (!other.TryGetComponent<Faction>(out var f))
            {
                f = other.GetComponentInParent<Faction>();
            }

            return f != null && f.Team == enemyTeam;
        }

        protected abstract Transform GetEnemyRoot(Collider other);
        protected abstract bool IsValidEnemy(Transform enemy);

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!IsEnemy(other))
            {
                return;
            }

            var t = GetEnemyRoot(other);
            if (t != null)
            {
                enemies.Add(t);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other == null)
            {
                return;
            }

            var t = GetEnemyRoot(other);
            if (t != null)
            {
                enemies.Remove(t);
            }
        }
    }
}
