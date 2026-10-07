using UnityEngine;

public class TreeDamageVisual : MonoBehaviour
{
    [Header("Tree Models(Stage0 -> Stage7)")]
    [SerializeField] private GameObject[] stageModels = new GameObject[8];

    [Header("Tree HP Rate to Change Model")]
    [SerializeField] private float[] hpThresholds = { 0.9f, 0.8f, 0.65f, 0.5f, 0.35f, 0.2f, 0.1f };

    private int currentStage = -1;

    public void UpdateVisual(float currentHP, float maxHp)
    {
        if (maxHp <= 0f) return;

        if (stageModels.Length != hpThresholds.Length + 1)
        {
            Debug.LogError("모델 개수는 체력 기준 개수보다 1개 많아야 한다.", this);
            return;
        }

        float hpRatio = Mathf.Clamp01(currentHP / maxHp);
        int nextStage = 0;

        for (int i = 0; i < hpThresholds.Length; i++)
        {
            if (hpRatio >= hpThresholds[i]) break;
            nextStage = i + 1;
        }

        if (currentStage == nextStage) return;

        for (int i = 0; i < stageModels.Length; i++)
        {
            if (stageModels[i] != null) stageModels[i].SetActive(i == nextStage);
        }

        currentStage = nextStage;
    }
}
