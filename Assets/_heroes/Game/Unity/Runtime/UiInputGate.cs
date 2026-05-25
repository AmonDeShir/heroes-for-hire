namespace Heroes.Game.Runtime
{
    public static class UiInputGate
    {
        public static bool CursorOnBlockingUi { get; private set; }

        public static void SetCursorOnBlockingUi(bool value)
        {
            CursorOnBlockingUi = value;
        }
    }
}
