namespace Heroes.Game.AI
{
    public static class GoapRuntimeConfig
    {
        public static GoapBuildingReferences Buildings { get; private set; }

        public static void Set(GoapBuildingReferences buildings)
        {
            Buildings = buildings;
        }
    }
}


