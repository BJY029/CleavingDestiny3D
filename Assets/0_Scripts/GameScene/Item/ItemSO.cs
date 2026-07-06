using System.Collections.Generic;
using UnityEngine;

//아이템 타입
public enum ItemType
{
    Damage, Defence, Heal, Gimmick
}

//아이템 희귀도
public enum ItemClass
{
    Common, Hero, Rare, Legendary
}

//아이템 적용 타겟
public enum ItemTarget
{
    Self, Opponent, Tree, SelfVillage, OpponentVillage, OpponentTree, Global
}

//아이템 효과 타입
public enum ItemEffect
{
    AddStatus,//StatusInstance 따로 생성
    DeltaTreeUp,
    DeltaVillageHp,
    DeltaPlayerEng,
    DeltaVillageShield,
    MultVillageShield,
    TransferOpponentShieldPct,
    NewDrugDevelopment,
    DisplayByTag,
}


//아이템 사용 시점 트리거
[System.Flags]
public enum TriggerMask
{
    None = 0,
    OnTurnStart = 1 << 0,       //턴 시작시 적용
    OnBeforeAttack = 1 << 1,    //공격 전 적용
    OnAfterAttack = 1 << 2,     //공격 후 적용
    OnDamageConvert = 1 << 3,   //데미지-방어력 변환 적용
    OnTurnEnd = 1 << 4,         //턴 종료시 적용
    OnVillageStart = 1 << 5,    //마을 페이즈 시작시 적용
    OnTreeDamage = 1 << 6,
}

//아이템 태그
[System.Flags]
public enum TagMask
{
    None = 0,
    Positive = 1 << 0,      //버프용 아이템(플레이어에게 이득인 효과)
    Negative = 1 << 1,      //디버프 아이템(플레이어 손해, '신의 가호' 같은 정화 아이템들이 해당 태그 달린 아이템 효과 제거)
    Curse = 1 << 2,         //저주 아이템
    Taunt = 1 << 3,         //도발 아이템
    Termite = 1 << 4,       //흰개미 아이템
    Hidden = 1 << 5,        //랜덤 아이템
    Counterable = 1 << 6,   //카운터용 아이템
}

//아이템 효과 적용 기간
public enum DurationType
{
    ThisTurn,   //이번 턴
    NextTurn,   //다음 턴까지
    Turns,      //N 턴
    UntilWaveEnd,//오늘
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemId;       //고유 ID
    public string displayName_ID;  //UI에서 보여지는 이름 CSV ID
    public string itemDesc_ID;     //아이템 설명 CSV ID
    public int itemCost;        //필요한 기력량
    public Sprite Icon
    {
        get
        {
            if (Application.isPlaying && AtlasManager.instance != null)
            {
                return AtlasManager.instance.GetItemSprite(itemId);
            }
            return null;
        }
    }
    public ItemType type;       //아이템 Type
    public ItemTarget target;   //아이템 적용 타겟
    public ItemClass itemClass; //아이템 등급

    public float itemWeight = 1f; //아이템 등장 확률(같은 등급 내 확률, 기본 1)

    //아이템 사용 제약
    public bool oncePerTurn;    //턴 당 한번
    public bool oncePerDay;     //하루 당 한번
    public bool oncePerGame;    //게임 당 한번

    public List<EffectSpec> effects; //아이템 이펙트들
}

[System.Serializable]
public class EffectSpec
{
    //아이템 효과 타입
    public ItemEffect effectType;

    //범용 파라미터
    public int intValue1;
    public int intValue2;
    public float floatValue1;
    public float floatValue2;


    //effectType이 AddStatus인 경우, 다음 객체로 타입 정의
    public StatusSpec statusSpce;
}

[System.Serializable]
public class StatusSpec
{
    public string statusId;             //아이템 ID
    public DurationType durationType;   //아이템 적용 기간
    public int durationTurns;           //durationType이 Turns인 경우, 해당 Turns 값
    public TriggerMask triggers;        //아이템 발동 시점
    public TagMask tags;                //아이템 태그

    public int priority;                //데미지 파이프라인 우선 순위

    public float multiplier;            //데미지 배수
    public float convertRate;
    public int flatValue;               //고정 데미지/회복/실드 등
    public int randMin, randMax;        //랜덤 범위
    public bool bypassConversion;       //흰개미 데미지 등 방어력으로 전환되지 않는 데미지
    public bool basicOnly;              //평타 데미지만 다음으로 적용
    public bool consumeOnTrigger;       //트리거 발생시 소비 후 삭제 할지 플래그
}
