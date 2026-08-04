using UnityEngine;
using PrimeTween;
using System;

public class FirstPersonTweenAnimator : MonoBehaviour
{
    // PlayerAnimationController로부터 전달받을 참조 변수들
    private Transform itemTransform;
    private MeshRenderer itemMeshRenderer;
    private Pose itemUsePoint;
    private MaterialPropertyBlock itemMpb;
    private Transform cameraAnimationPivot; // 카메라 회전 연출용 피벗

    // Tween 제어용 시퀀스
    private Sequence actionSequence;

    private bool isPerformingAction = false;
    private bool isInitialized = false;

    public void Initialize(Transform axe, Transform item, MeshRenderer itemMesh, Pose usePoint, MaterialPropertyBlock mpb, Transform cameraPivot)
    {
        itemTransform = item;
        itemMeshRenderer = itemMesh;
        itemUsePoint = usePoint;
        itemMpb = mpb;
        cameraAnimationPivot = cameraPivot;

        isInitialized = true;
    }

    /// <summary>
    /// [아이템 사용 애니메이션] 아이템을 가운데로 띄워 소모하는 연출
    /// </summary>
    public void PlayUseItemAnimation(Transform itemSlotTransform, ItemClass currentItemClass, Texture itemTexture, Action onCompleteCallback)
    {
        if (!isInitialized || isPerformingAction || itemTransform == null) return;
        isPerformingAction = true;

        if (actionSequence.isAlive) actionSequence.Stop();

        // 렌더러가 비활성화되어 있는 경우 다시 켬 (DisableFirstPersonRenderersOnly로 꺼진 상태 대비)
        if (itemMeshRenderer != null)
        {
            itemMeshRenderer.enabled = true;
        }

        // Material Property Block 셋업
        if (itemMeshRenderer != null && itemMpb != null)
        {
            itemMeshRenderer.GetPropertyBlock(itemMpb);
            if (itemTexture != null)
            {
                itemMpb.SetTexture("_BaseMap", itemTexture);
                itemMpb.SetColor("_BaseColor", Color.white);
            }
            itemMeshRenderer.SetPropertyBlock(itemMpb);
        }

        // 아이템 초기 위치 & 스케일 세팅
        itemTransform.localScale = Vector3.one;
        itemTransform.gameObject.SetActive(true);
        itemTransform.SetPositionAndRotation(itemSlotTransform.position, itemSlotTransform.rotation);

        Vector3 worldExplosionPosition = itemTransform.parent != null 
            ? itemTransform.parent.TransformPoint(itemUsePoint.position) 
            : itemTransform.position;
        Debug.Log($"[FirstPersonTweenAnimator] Explosion Position: {worldExplosionPosition}");

        actionSequence = Sequence.Create()
            .Group(Tween.LocalPosition(itemTransform, itemUsePoint.position, 0.5f, Ease.InOutQuad))
            .Group(Tween.LocalRotation(itemTransform, itemUsePoint.rotation, 0.5f, Ease.InOutQuad))

            .Chain(Tween.Scale(itemTransform, Vector3.one * 1.5f, 0.4f, Ease.OutBack))
            .Group(Tween.ShakeLocalRotation(itemTransform, new Vector3(0, 15f, 0), 0.4f))
            .Chain(Tween.Scale(itemTransform, Vector3.zero, 0.4f, Ease.InBack))
            .OnComplete(() =>
            {
                VFXManager.Instance.PlayItemExplosion(worldExplosionPosition, currentItemClass);
                itemTransform.gameObject.SetActive(false);

                if (itemMeshRenderer != null)
                {
                    itemMeshRenderer.enabled = false;
                }

                isPerformingAction = false;
                onCompleteCallback?.Invoke();
            });
    }

    private void OnDestroy()
    {
        if (actionSequence.isAlive) actionSequence.Stop();
    }
}
