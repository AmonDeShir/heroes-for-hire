using EventBus;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Quests;
using Heroes.Game.Buildings;
using Heroes.Game.Monsters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace Heroes.Game.Runtime
{
    public sealed class QuestController : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private int defaultCombatGold = 10;

        private QuestService _quests;
        private SelectionService _selection;

        private InputAction _click;

        private bool _armed;
        private QuestType _armedType;

        public bool IsCombatArmed => _armed && _armedType == QuestType.Combat;

        [Inject]
        public void Construct(QuestService quests, SelectionService selection)
        {
            _quests = quests;
            _selection = selection;
            QuestRuntimeConfig.Set(quests);
        }

        public void ArmCombatQuest()
        {
            _armed = true;
            _armedType = QuestType.Combat;
        }

        private void OnEnable()
        {
            _click ??= new InputAction("QuestClick", InputActionType.Button, "<Mouse>/leftButton");
            _click.performed += OnClick;
            _click.Enable();
        }

        private void OnDisable()
        {
            if (_click != null)
            {
                _click.performed -= OnClick;
                _click.Disable();
            }
        }

        private void OnClick(InputAction.CallbackContext ctx)
        {
            if (!_armed || _armedType != QuestType.Combat)
            {
                return;
            }

            if (worldCamera == null || Mouse.current == null || _quests == null)
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
                return;
            }

            var go = hit.collider != null ? hit.collider.gameObject : null;
            if (go == null)
            {
                return;
            }

            var building = go.GetComponentInParent<BuildingFacade>();
            if (building != null)
            {
                if (_quests.TryCreateCombatQuestForTarget(building.Id, QuestTargetKind.Building, defaultCombatGold, out _))
                {
                    _selection?.Select(building);
                }
                _armed = false;
                return;
            }

            var monster = go.GetComponentInParent<MonsterFacade>();
            if (monster != null)
            {
                if (_quests.TryCreateCombatQuestForTarget(monster.InstanceId, QuestTargetKind.Monster, defaultCombatGold, out _))
                {
                    _selection?.Select(monster);
                }
                _armed = false;
                return;
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
