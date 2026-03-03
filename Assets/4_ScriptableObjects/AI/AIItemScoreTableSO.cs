using UnityEngine;

[CreateAssetMenu(fileName = "AIItemScoreTableSO", menuName = "Scriptable Objects/AIItemScoreTableSO")]
public class AIItemScoreTableSO : ScriptableObject
{
    [Header("희귀도 기본 가중치")]
    public float classCommon = 10f;
    public float classHero = 30f;
    public float classRare = 60f;
    public float classLegendary = 100f;

    [Header("고정 점수")]
    public float healEnergyScore = 200f;

    [Header("제약 조건 페널티")]
    public float impossibleCostPenalty = -500f;
    public float lackEnergyPenalty = -80f;
    public float fullInvPenalty = -200f;

    [Tooltip("인벤토리에 동일한 아이템 수에 적용될 최대 패널티(3개가 최대 패널티)")]
    public float duplicateItemPenalty = -120f;

    [Header("곡선 평가 최대 가중치(Max Weights)")]
    [Tooltip("내 마을 체력이 0에 수렴할 때 마을 방어 템이 받는 최대 점수")]
    public float defVillageMaxScore = 100f;

    [Tooltip("나무 체력이 0에 수렴할 때 나무 힐 템이 받는 최대 점수")]
    public float healTreeMaxScore = 150f;

    [Tooltip("나무 체력이 1(100%)에 수렴할 때 공격템이 받는 최대 점수")]
    public float dmgTreeMaxScore = 100f;

    [Tooltip("기믹 카운터 가중치")]
    public float purifyBonus = 200f;
    public float killCatchBonus = 150f;
    public float gimmicBonus = 100f;
}
