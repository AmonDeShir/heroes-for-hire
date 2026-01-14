using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMode : MonoBehaviour
{
    [SerializeField]
    private BuildingData m_defaultSelection;
    
    public BuildingData Selected { get; set; }

    private InputAction _clickAction;
    
    public void Start()
    {
        Selected = m_defaultSelection;
        _clickAction = InputSystem.actions.FindAction("Attack");
    }

    public void Update()
    {
        if (!IsUserBuilding() || !Selected)
        {
            return;
        }

        var ray = GetCursorPositionInWord();

        if (Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Terrain")))
        {
            SpawnBuilding(hit.point);
        }
    }
    
    private bool IsUserBuilding()
    {
        return _clickAction.triggered;
    }

    private Ray GetCursorPositionInWord()
    {
        return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    private void SpawnBuilding(Vector3 position)
    {
        Instantiate(Selected.Prefab, position, Selected.Prefab.transform.rotation);
    }
}
