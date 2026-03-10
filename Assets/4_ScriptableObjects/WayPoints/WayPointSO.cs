using UnityEngine;

[CreateAssetMenu(fileName = "WayPointSO", menuName = "Scriptable Objects/WayPointSO")]
public class WayPointSO : ScriptableObject
{
    [Header("인벤토리 위치 값")]
    public Vector3 Inv_1 = new Vector3(17f, 0f, 0f);
    public Vector3 Inv_2 = new Vector3(-17f, 0f, 0f);

    [Header("인벤토리 입구 위치 값")]
    public Vector3 Inv_Entry_1 = new Vector3(10f, 0f, 0f);
    public Vector3 Inv_Entry_2 = new Vector3(10f, 0f, 0f);

    [Header("Hit 위치 값")]
    public Vector3 Hit_1 = new Vector3(3f, 0f, 0f);
    public Vector3 Hit_2 = new Vector3(-3f, 0f, 0f);
}
