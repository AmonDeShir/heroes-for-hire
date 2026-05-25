namespace Heroes.Game.Combat
{
    public static class CombatRuntimeConfig
    {
        public static CombatService Service { get; private set; }

        public static void Set(CombatService service)
        {
            Service = service;
        }
    }
}
