namespace Heroes.Game.Combat
{
    public enum HeroCombatState
    {
        Idle = 0,
        Approach = 1,
        AttackWindup = 2,
        AttackRecover = 3,
        TryHeal = 4,
        TryBoostBeforeFlee = 5,
        Flee = 6,
        Dead = 7,
    }
}
