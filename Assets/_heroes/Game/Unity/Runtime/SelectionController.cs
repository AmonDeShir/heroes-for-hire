using System;
using Heroes.Game.Abstractions;
using Heroes.Game.Buildings;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public class SelectionController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        
        public SelectionService SelectionService { get; private set; }

        [Inject]
        public void Construct(SelectionService selectionService)
        {
            SelectionService = selectionService;
        }

        private void Update()
        {
            HandleCancel();
            HandleSelection();
        }
        
        
        private void HandleCancel()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SelectionService.Clear();
                return;
            }

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                SelectionService.Clear();
            }
        }
        
        private void HandleSelection()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }
            
            var ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, 500f))
            {
                SelectionService.Clear();
                return;
            }

            if (hit.collider.gameObject.TryGetComponent<ISelectable>(out var selectable))
            {
                SelectionService.Select(selectable);
            }
        }
    }
}