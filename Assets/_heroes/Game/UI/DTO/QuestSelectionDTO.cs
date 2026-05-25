namespace Heroes.Presentation.UI.SelectionPanel
{
    public sealed class QuestParticipantDTO
    {
        public string HeroId;
        public string Icon;

        public QuestParticipantDTO(string heroId, string icon)
        {
            HeroId = heroId;
            Icon = icon;
        }
    }

    public sealed class QuestSelectionDTO
    {
        public string QuestId;
        public int PoolGold;
        public bool CanIncrease;
        public QuestParticipantDTO[] Participants;

        public QuestSelectionDTO(string questId, int poolGold, bool canIncrease, QuestParticipantDTO[] participants)
        {
            QuestId = questId;
            PoolGold = poolGold;
            CanIncrease = canIncrease;
            Participants = participants ?? System.Array.Empty<QuestParticipantDTO>();
        }
    }
}
