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
        if (!_clickAction.triggered || !Selected)
        {
            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Terrain")))
        {
            Instantiate(Selected.Prefab, hit.point, Selected.Prefab.transform.rotation);
        }
    }
}
