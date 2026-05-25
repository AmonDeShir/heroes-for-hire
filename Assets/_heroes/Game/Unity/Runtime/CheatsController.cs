using Heroes.Game.Abstractions;
using Heroes.Game.Buildings;
using Heroes.Game.Heroes;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public sealed class CheatsController : MonoBehaviour
    {
        [SerializeField] private bool enabledCheats;

        private SelectionService _selection;
        private KingdomService _kingdom;

        private InputAction _gold;
        private InputAction _pop;
        private InputAction _damage;

        [Inject]
        public void Construct(SelectionService selection, KingdomService kingdom)
        {
            _selection = selection;
            _kingdom = kingdom;
        }

        private void OnEnable()
        {
            if (!enabledCheats)
            {
                return;
            }

            _gold ??= new InputAction("CheatGold", InputActionType.Button, "<Keyboard>/g");
            _pop ??= new InputAction("CheatPop", InputActionType.Button, "<Keyboard>/h");
            _damage ??= new InputAction("CheatDamage", InputActionType.Button, "<Keyboard>/d");

            _gold.performed += OnGold;
            _pop.performed += OnPop;
            _damage.performed += OnDamage;

            _gold.Enable();
            _pop.Enable();
            _damage.Enable();
        }

        private void OnDisable()
        {
            if (_gold != null)
            {
                _gold.performed -= OnGold;
                _gold.Disable();
            }

            if (_pop != null)
            {
                _pop.performed -= OnPop;
                _pop.Disable();
            }

            if (_damage != null)
            {
                _damage.performed -= OnDamage;
                _damage.Disable();
            }
        }

        private static bool IsAltDown()
        {
            return Keyboard.current != null && (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed);
        }

        private void OnGold(InputAction.CallbackContext _)
        {
            if (!enabledCheats || !IsAltDown())
            {
                return;
            }

            if (_selection?.Selected is HeroFacade hero)
            {
                hero.AddGold(100);
                return;
            }

            _kingdom?.AddGold(1000);
        }

        private void OnPop(InputAction.CallbackContext _)
        {
            if (!enabledCheats || !IsAltDown())
            {
                return;
            }

            _kingdom?.AddPeople(10);
        }

        private void OnDamage(InputAction.CallbackContext _)
        {
            if (!enabledCheats || !IsAltDown())
            {
                return;
            }

            if (_selection?.Selected is IDamageable d && d.IsAlive)
            {
                d.ApplyDamage(50f);
            }
        }
    }
}
