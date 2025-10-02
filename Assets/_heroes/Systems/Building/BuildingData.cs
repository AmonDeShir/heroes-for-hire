using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Data/Building")]
public class BuildingData : ScriptableObject
{
    [SerializeField]
    private string m_name;

    [SerializeField]
    private float m_hp;

    [SerializeField]
    private int m_cost;

    [SerializeField]
    private GameObject m_prefab;

    public string Name => m_name;
    public float HP => m_hp;
    public int Cost => m_cost;
    public GameObject Prefab => m_prefab;
}
