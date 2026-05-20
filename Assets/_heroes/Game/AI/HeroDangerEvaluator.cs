using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class HeroDangerEvaluator : MonoBehaviour
    {
        [SerializeField] private LayerMask overlapMask = ~0;

        public float Evaluate(HeroFacade hero)
        {
            if (hero == null || hero.Model == null || !hero.Model.IsAlive)
            {
                return 0f;
            }

            if (hero.Model.IsInHome)
            {
                return 0f;
            }

            var colliders = Physics.OverlapSphere(hero.transform.position, hero.Model.DangerSenseRadius, overlapMask);
            var enemyPower = 0f;
            var friendlyPower = 0f;

            foreach (var hit in colliders)
            {
                if (hit == null)
                {
                    continue;
                }

                if (!hit.TryGetComponent<Faction>(out var faction))
                {
                    faction = hit.GetComponentInParent<Faction>();
                }

                if (faction == null)
                {
                    continue;
                }

                if (faction.gameObject == hero.gameObject)
                {
                    friendlyPower += Mathf.Max(1, faction.Level);
                    continue;
                }

                if (faction.Team == TeamType.Enemies)
                {
                    enemyPower += Mathf.Max(1, faction.Level);
                }
                else
                {
                    friendlyPower += Mathf.Max(1, faction.Level);
                }
            }

            return Mathf.Clamp01((enemyPower - friendlyPower) / 5f);
        }
    }
}


