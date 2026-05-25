namespace Heroes.Presentation.UI.SelectionPanel
{
    public sealed class CombatSelectionDTO
    {
        public string LeftId;
        public string LeftName;
        public string LeftDescription;
        public string LeftIcon;
        public float LeftHp;
        public float LeftMaxHp;

        public string RightId;
        public string RightName;
        public string RightDescription;
        public string RightIcon;
        public float RightHp;
        public float RightMaxHp;

        public CombatSelectionDTO(
            string leftId,
            string leftName,
            string leftDescription,
            string leftIcon,
            float leftHp,
            float leftMaxHp,
            string rightId,
            string rightName,
            string rightDescription,
            string rightIcon,
            float rightHp,
            float rightMaxHp)
        {
            LeftId = leftId;
            LeftName = leftName;
            LeftDescription = leftDescription;
            LeftIcon = leftIcon;
            LeftHp = leftHp;
            LeftMaxHp = leftMaxHp;

            RightId = rightId;
            RightName = rightName;
            RightDescription = rightDescription;
            RightIcon = rightIcon;
            RightHp = rightHp;
            RightMaxHp = rightMaxHp;
        }
    }
}
