namespace Heroes.Game.Quests
{
    public static class QuestRuntimeConfig
    {
        public static QuestService Service { get; private set; }

        public static void Set(QuestService service)
        {
            Service = service;
        }
    }
}
