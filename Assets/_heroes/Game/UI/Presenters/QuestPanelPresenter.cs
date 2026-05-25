using Heroes.Game.Runtime;
using OneJS;
using UnityEngine;

namespace Heroes.Presentation.UI.QuestPanel
{
    public partial class QuestPanelPresenter : MonoBehaviour
    {
        [SerializeField] private QuestController questController;

        [EventfulProperty] private bool _combatArmed;

        public void ArmCombatQuest()
        {
            if (questController == null)
            {
                return;
            }

            questController.ArmCombatQuest();
            CombatArmed = true;
        }

        private void Update()
        {
            if (questController == null)
            {
                return;
            }

            if (CombatArmed != questController.IsCombatArmed)
            {
                CombatArmed = questController.IsCombatArmed;
            }
        }

        public void ClearArmed()
        {
            CombatArmed = false;
        }
    }
}
