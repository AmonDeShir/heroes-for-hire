namespace Heroes.Game.Core.Health
{
    public sealed class DamageLogic
    {
        private readonly HealthModel _health;

        public DamageLogic(HealthModel health)
        {
            _health = health;
        }

        public void Apply(float amount)
        {
            if (_health == null)
            {
                return;
            }

            if (amount <= 0f)
            {
                return;
            }

            var next = _health.Current - amount;
            _health.SetCurrent(next);
        }
    }
}
