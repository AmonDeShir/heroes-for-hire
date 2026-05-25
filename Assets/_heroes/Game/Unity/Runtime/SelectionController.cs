using System;
using Heroes.Game.Abstractions;
using Heroes.Game.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public class SelectionController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        private InputAction _select;
        private InputAction _clear;
        private InputAction _cancel;
        
        public SelectionService SelectionService { get; private set; }
        [Inject]
        public void Construct(SelectionService selectionService)
        {
            SelectionService = selectionService;
        }

        private void OnEnable()
        {
            _select ??= new InputAction("Select", InputActionType.Button, "<Mouse>/leftButton");
            _clear ??= new InputAction("Clear", InputActionType.Button, "<Mouse>/rightButton");
            _cancel ??= new InputAction("Cancel", InputActionType.Button, "<Keyboard>/escape");

            _select.performed += OnSelect;
            _clear.performed += OnClear;
            _cancel.performed += OnClear;

            _select.Enable();
            _clear.Enable();
            _cancel.Enable();
        }

        private void OnDisable()
        {
            if (_select != null)
            {
                _select.performed -= OnSelect;
                _select.Disable();
            }

            if (_clear != null)
            {
                _clear.performed -= OnClear;
                _clear.Disable();
            }

            if (_cancel != null)
            {
                _cancel.performed -= OnClear;
                _cancel.Disable();
            }

        }

        private void OnClear(InputAction.CallbackContext _)
        {
            SelectionService.Clear();
        }

        private void OnSelect(InputAction.CallbackContext _)
        {
            if (worldCamera == null || Mouse.current == null)
            {
                return;
            }

            if (IsPointerOverUi())
            {
                return;
            }

            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, 500f, ~0, QueryTriggerInteraction.Ignore))
            {
                SelectionService.Clear();
                return;
            }

            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<ISelectable>(out var selectable))
            {
                SelectionService.Select(selectable);
            }
        }

        private static bool IsPointerOverUi()
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current == null)
                {
                    return false;
                }
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }

            return UiInputGate.CursorOnBlockingUi;
        }
    }
}

