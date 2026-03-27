namespace Heroes.Game.Core.Events
{
    public readonly struct ResourcesChangedEvent
    {
        public int Gold { get; }

        public ResourcesChangedEvent(int gold)
        {
            Gold = gold;
        }
    }
}