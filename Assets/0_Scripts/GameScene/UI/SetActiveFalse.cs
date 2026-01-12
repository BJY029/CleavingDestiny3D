using UnityEngine;

public class SetActiveFalse : MonoBehaviour
{
    public void SetEnable()
    {
        this.gameObject.SetActive(false);
    }

    public void SetDestory()
    {
        Destroy(gameObject);
    }
}
