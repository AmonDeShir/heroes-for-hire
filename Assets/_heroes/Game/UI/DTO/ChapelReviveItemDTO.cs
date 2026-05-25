namespace Heroes.Presentation.UI.SelectionPanel
{
    public sealed class ChapelReviveItemDTO
    {
        public string HeroId;
        public string Icon;
        public float RemainingSeconds;
        public float TotalSeconds;

        public ChapelReviveItemDTO(string heroId, string icon, float remainingSeconds, float totalSeconds)
        {
            HeroId = heroId;
            Icon = icon;
            RemainingSeconds = remainingSeconds;
            TotalSeconds = totalSeconds;
        }
    }
}
