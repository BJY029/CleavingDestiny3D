using UnityEngine;

public class PlayerEffectPoints : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Transform foot;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform axe;

    public Transform Head => head;
    public Transform Body => body;
    public Transform Foot => foot;
    public Transform RightHand => rightHand;
    public Transform LeftHand => LeftHand;
    public Transform Axe => axe;
}
