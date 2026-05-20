using System.Collections.Generic;
using Heroes.Game.Core.Health;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Game.Runtime
{
    public sealed class TimedEffectRunner : MonoBehaviour
    {
        private sealed class Dot
        {
            public float Dps;
            public float Remaining;
        }

        private readonly List<Dot> _dots = new();
        private DamageLogic _damage;

        private sealed class StatBuff
        {
            public float Attack;
            public float Defence;
            public float Speed;
            public float Remaining;
        }

        private sealed class RegenBuff
        {
            public float AddPerSecond;
            public float Remaining;
        }

        private readonly List<StatBuff> _statBuffs = new();
        private readonly List<RegenBuff> _regenBuffs = new();
        private HeroFacade _hero;

        public void Initialize(HeroFacade hero, DamageLogic damageLogic)
        {
            _hero = hero;
            _damage = damageLogic;
        }

        public void AddDamageOverTime(float dps, float duration)
        {
            if (dps <= 0f || duration <= 0f)
            {
                return;
            }

            _dots.Add(new Dot { Dps = dps, Remaining = duration });
        }

        public void AddTimedStatBuff(float attack, float defence, float speed, float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            if (Mathf.Approximately(attack, 0f) && Mathf.Approximately(defence, 0f) && Mathf.Approximately(speed, 0f))
            {
                return;
            }

            _statBuffs.Add(new StatBuff
            {
                Attack = attack,
                Defence = defence,
                Speed = speed,
                Remaining = duration,
            });
        }

        public void AddTimedHpRegeneration(float addPerSecond, float duration)
        {
            if (duration <= 0f || Mathf.Approximately(addPerSecond, 0f))
            {
                return;
            }

            _regenBuffs.Add(new RegenBuff { AddPerSecond = addPerSecond, Remaining = duration });
        }

        private void Update()
        {
            var dt = Time.deltaTime;

            if (_damage != null && _dots.Count > 0)
            {
                for (var i = _dots.Count - 1; i >= 0; i--)
                {
                    var dot = _dots[i];
                    dot.Remaining -= dt;
                    _damage.Apply(dot.Dps * dt);

                    if (dot.Remaining <= 0f)
                    {
                        _dots.RemoveAt(i);
                    }
                }
            }

            if (_hero?.Model != null)
            {
                var atk = 0f;
                var def = 0f;
                var spd = 0f;
                var regen = 0f;

                for (var i = _statBuffs.Count - 1; i >= 0; i--)
                {
                    var buff = _statBuffs[i];
                    buff.Remaining -= dt;
                    atk += buff.Attack;
                    def += buff.Defence;
                    spd += buff.Speed;

                    if (buff.Remaining <= 0f)
                    {
                        _statBuffs.RemoveAt(i);
                    }
                }

                for (var i = _regenBuffs.Count - 1; i >= 0; i--)
                {
                    var buff = _regenBuffs[i];
                    buff.Remaining -= dt;
                    regen += buff.AddPerSecond;
                    if (buff.Remaining <= 0f)
                    {
                        _regenBuffs.RemoveAt(i);
                    }
                }

                _hero.Model.SetTimedBonuses(atk, def, spd);
                _hero.Model.SetTimedHpRegeneration(regen);
            }
        }
    }
}


