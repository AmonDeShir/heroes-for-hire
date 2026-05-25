using Heroes.Game.Runtime;
using OneJS;
using UnityEngine;

namespace Heroes.Presentation.UI.Input
{
    public partial class UiInputGatePresenter : MonoBehaviour
    {
        [EventfulProperty] private bool _cursorOnUi;

        public void SetCursorOnUi(bool value)
        {
            CursorOnUi = value;
            UiInputGate.SetCursorOnBlockingUi(value);
        }
    }
}
