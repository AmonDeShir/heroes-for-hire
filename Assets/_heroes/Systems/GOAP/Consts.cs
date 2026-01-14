namespace GOAP
{
    public static class Consts
    {
        public static class Beliefs
        {
            public const string NOTHING = "Nothing";

            public const string AGENT_IDLE = "AgentIdle";
            public const string AGENT_MOVING = "AgentMoving";
            public const string AGENT_STAMINA_OK = "AgentStaminaOk";
            public const string AGENT_HEALTH_LOW = "AgentHealthLow";
            public const string AGENT_IS_HEALTHY = "AgentIsHealthy";

            public const string AGENT_IS_TIRED = "AgentIsTired";
            public const string AGENT_STAMINA_LOW = "AgentStaminaLow";
            public const string AGENT_IS_RESTED = "AgentIsRested";

            public const string AGENT_AT_HOME = "AgentAtHome";
            public const string AGENT_AT_WORK = "AgentAtWork";
            public const string AGENT_AT_SHOP = "AgentAtShop";
            public const string AGENT_AT_MINE = "AgentAtMine";

            public const string HAS_SWORD = "HasSword";
            public const string HAS_PICKAXE = "HasPickaxe";
            public const string HAS_COFFEE = "HasCoffee";
            public const string HAS_ENOUGH_GOLD_FOR_SWORD = "HasEnoughGoldForSword";
            public const string HAS_ENOUGH_GOLD_FOR_PICKAXE = "HasEnoughGoldForPickaxe";
            public const string HAS_ENOUGH_GOLD_FOR_COFFEE = "HasEnoughGoldForCoffee";
        }

        public static class Actions
        {
            public const string RELAX = "Relax";
            public const string WANDER_AROUND = "Wander Around";
            
            public const string GO_MINE = "Go Mine";
            public const string MINE_FOR_GOLD = "Mine For Gold";

            public const string BUY_PICKAXE = "Buy Pickaxe";
            public const string BUY_COFFEE = "Buy Coffee";
            public const string DRINK_COFFEE = "Drink Coffee";

            public const string GO_HOME = "Go Home";
            public const string REST_AT_HOME = "Rest At Home";

            public const string GO_WORK = "Go Work";
            public const string WORK_FOR_GOLD = "Work For Gold";

            public const string GO_TO_SHOP = "Go To Shop";
            public const string BUY_SWORD = "Buy Sword";
        }

        public static class Goals
        {
            public const string GET_RESTED = "GetRested";
            public const string HAVE_SWORD = "HaveSword";
            public const string KEEP_HEALTH_UP = "KeepHealthUp";
            public const string WANDER = "Wander";
            public const string CHILL_OUT = "ChillOut";
            public const string GET_STAMINA_OK = "GetStaminaOk";
            public const string HAVE_PICKAXE = "HavePickaxe";
        }
    }
}