namespace Heroes.Game.Core.Health
{
    public sealed class HealLogic
    {
        private readonly HealthModel _health;

        public HealLogic(HealthModel health)
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

            var next = _health.Current + amount;
            _health.SetCurrent(next);
        }
    }
}
