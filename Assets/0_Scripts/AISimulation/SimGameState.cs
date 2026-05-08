using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatType { VillageHP, Energy, Barrier, TotalDmg, MultRate, BarConRate }
public enum TreeType { TreeHP, TreeToxic }
public class SimGameState
{
    public SimVillageState simVillageState;
    public static event Action<int, StatType, float> OnStatChange;
    public static event Action<TreeType, float> OnTreeChange;

    public static event Action<int, string> OnItemAdded;
    public static event Action<int, string> OnItemRemoved;

    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;
    public int roomSeed;

    public int looserPlayerNum;
    public int curTurnPlayerNum;

    public int turn;
    public int wave;
    public int day;

    public int totalTurnCount;
    private float _p1TotalHitDmg;
    public float p1TotalHitDmg
    {
        get => _p1TotalHitDmg;
        set
        {
            if (Mathf.Approximately(_p1TotalHitDmg, value)) return;
            _p1TotalHitDmg = value;
            OnStatChange?.Invoke(1, StatType.TotalDmg, _p1TotalHitDmg);
        }
    }

    private float _p1VillHP;
    public float p1VillHP
    {
        get => _p1VillHP;
        set
        {
            if (Mathf.Approximately(_p1VillHP, value)) return;
            _p1VillHP = value;
            OnStatChange?.Invoke(1, StatType.VillageHP, _p1VillHP);
        }
    }

    private float _p1VillBarrier;
    public float p1VillBarrier
    {
        get => _p1VillBarrier;
        set
        {
            if (Mathf.Approximately(_p1VillBarrier, value)) return;
            _p1VillBarrier = value;
            OnStatChange?.Invoke(1, StatType.Barrier, _p1VillBarrier);
        }
    }

    private float _p1VillBarConRate;
    public float p1VillBarConRate
    {
        get => _p1VillBarConRate;
        set
        {
            if (Mathf.Approximately(_p1VillBarConRate, value)) return;
            _p1VillBarConRate = value;
            OnStatChange?.Invoke(1, StatType.BarConRate, _p1VillBarConRate);
        }
    }

    private float _p1DmgMultRate;
    public float p1DmgMultRate
    {
        get => _p1DmgMultRate;
        set
        {
            if (Mathf.Approximately(_p1DmgMultRate, value)) return;
            _p1DmgMultRate = value;
            OnStatChange?.Invoke(1, StatType.MultRate, _p1DmgMultRate);
        }
    }

    private int _p1Energy;
    public int p1Energy
    {
        get => _p1Energy;
        set
        {
            if (_p1Energy == value) return;
            _p1Energy = value;
            OnStatChange?.Invoke(1, StatType.Energy, _p1Energy);
        }
    }

    private float _p2TotalHitDmg;
    public float p2TotalHitDmg
    {
        get => _p2TotalHitDmg;
        set
        {
            if (Mathf.Approximately(_p2TotalHitDmg, value)) return;
            _p2TotalHitDmg = value;
            OnStatChange?.Invoke(2, StatType.TotalDmg, _p2TotalHitDmg);
        }
    }

    private float _p2VillHP;
    public float p2VillHP
    {
        get => _p2VillHP;
        set
        {
            if (Mathf.Approximately(_p2VillHP, value)) return;
            _p2VillHP = value;
            OnStatChange?.Invoke(2, StatType.VillageHP, _p2VillHP);
        }
    }

    private float _p2VillBarrier;
    public float p2VillBarrier
    {
        get => _p2VillBarrier;
        set
        {
            if (Mathf.Approximately(_p2VillBarrier, value)) return;
            _p2VillBarrier = value;
            OnStatChange?.Invoke(2, StatType.Barrier, _p2VillBarrier);
        }
    }

    private float _p2VillBarConRate;
    public float p2VillBarConRate
    {
        get => _p2VillBarConRate;
        set
        {
            if (Mathf.Approximately(_p2VillBarConRate, value)) return;
            _p2VillBarConRate = value;
            OnStatChange?.Invoke(2, StatType.BarConRate, _p2VillBarConRate);
        }
    }

    private float _p2DmgMultRate;
    public float p2DmgMultRate
    {
        get => _p2DmgMultRate;
        set
        {
            if (Mathf.Approximately(_p2DmgMultRate, value)) return;
            _p2DmgMultRate = value;
            OnStatChange?.Invoke(2, StatType.MultRate, _p2DmgMultRate);
        }
    }

    private int _p2Energy;
    public int p2Energy
    {
        get => _p2Energy;
        set
        {
            if (_p2Energy == value) return;
            _p2Energy = value;
            OnStatChange?.Invoke(2, StatType.Energy, _p2Energy);
        }
    }


    private float _treeHP;
    public float treeHP
    {
        get => _treeHP;
        set
        {
            if (Mathf.Approximately(_treeHP, value)) return;
            _treeHP = value;
            OnTreeChange?.Invoke(TreeType.TreeHP, _treeHP);
        }
    }

    private float _treeToxicDmg;
    public float treeToxicDmg
    {
        get => _treeToxicDmg;
        set
        {
            if (Mathf.Approximately(_treeToxicDmg, value)) return;
            _treeToxicDmg = value;
            OnTreeChange?.Invoke(TreeType.TreeToxic, _treeToxicDmg);
        }
    }



    public bool p1HasDebuff, p2HasDebuff;


    public int p1LockCnt, p2LockCnt;
    public int p1LockpickCnt, p2LockpickCnt;
    public List<string> p1Inventory = new List<string>();
    public List<string> p2Inventory = new List<string>();

    //생성자
    public SimGameState(PlayerSetting playerSetting, RoomSetting roomSetting, VillageBalanceData villageBalanceData, VillageLevelData[] villageLevelDatas)
    {
        this.playerSetting = playerSetting;
        this.roomSetting = roomSetting;

        //simVillageState를 가장 먼저 생성해야 그 안의 스탯을 빼옴
        this.simVillageState = new SimVillageState(villageBalanceData, villageLevelDatas);

        //2. 초기 기력과 방어력을 업그레이드 관리 객체(simVillageState)에서 가져옴
        this.p1Energy = simVillageState.p1DayEnergy;
        this.p2Energy = simVillageState.p1DayEnergy;

        this.p1VillBarrier = simVillageState.p1BasicVillageBarrier;
        this.p2VillBarrier = simVillageState.p2BasicVillageBarrier;

        this.p1TotalHitDmg = this.p2TotalHitDmg = 0f;
        this.p1VillHP = this.p2VillHP = playerSetting.villageHP;
        this.p1VillBarConRate = this.p2VillBarConRate = playerSetting.barrierConversionRate;
        this.p1DmgMultRate = this.p2DmgMultRate = 1f;
        this.treeHP = roomSetting.treeHP;
        this.treeToxicDmg = roomSetting.treeAtkPow;

        this.turn = 0;
        this.wave = roomSetting.initialWave;
        this.day = roomSetting.startDay;

        this.p1LockCnt = this.p2LockCnt = 1;
        this.p1LockpickCnt = this.p2LockpickCnt = 0;
        this.roomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        this.totalTurnCount = 0;

        AISimUIController.instance.InitStatUI(this);
    }

    //플레이어 상태 DTO로 반환
    public LLMGameStateDTO GetStateForPlayer(int myPlayerNum)
    {
        LLMGameStateDTO dto = new LLMGameStateDTO();

        dto.currentWave = this.wave;
        dto.expectedToxicDamage = this.treeToxicDmg;
        dto.treeHP = this.treeHP;

        if (myPlayerNum == 1)
        {
            dto.myStatus = CreatePlayerDTO(simVillageState.p1MaxHitDmg, simVillageState.p1MinHitDmg, p1VillHP, p1VillBarrier, simVillageState.p1MaxEnergy, p1Energy, p1HasDebuff);
            dto.oppStatus = CreatePlayerDTO(simVillageState.p2MaxHitDmg, simVillageState.p2MinHitDmg, p2VillHP, p2VillBarrier, simVillageState.p2MaxEnergy, p2Energy, p2HasDebuff);
            dto.myInventory = CreateInventoryDTO(p1Inventory);
            dto.oppInventory = CreateInventoryDTO(p2Inventory);
            //dto.myVStatus = CreatePlayerVDTO(simVillageState.p1VillGold, simVillageState.P1VillageObjInfos);
        }
        else
        {
            dto.myStatus = CreatePlayerDTO(simVillageState.p2MaxHitDmg, simVillageState.p2MinHitDmg, p2VillHP, p2VillBarrier, simVillageState.p2MaxEnergy, p2Energy, p2HasDebuff);
            dto.oppStatus = CreatePlayerDTO(simVillageState.p1MaxHitDmg, simVillageState.p1MinHitDmg, p1VillHP, p1VillBarrier, simVillageState.p1MaxEnergy, p1Energy, p1HasDebuff);
            dto.myInventory = CreateInventoryDTO(p2Inventory);
            dto.oppInventory = CreateInventoryDTO(p1Inventory);
            // dto.myVStatus = CreatePlayerVDTO(simVillageState.p2VillGold, simVillageState.P2VillageObjInfos);
        }
        return dto;
    }

    //플레이어 정보를 담은 DTO 구성
    private PlayerStatusDTO CreatePlayerDTO(int maxTreeHitDmg, int minTreeHitDmg, float hp, float barrier, int maxEnergy, int energy, bool Debuff)
    {
        return new PlayerStatusDTO
        {
            maxTreeHitDmg = maxTreeHitDmg,
            minTreeHitDmg = minTreeHitDmg,
            villageHP = hp,
            barrier = barrier,
            maxEnergy = maxEnergy,
            energy = energy,
            hasDebuff = Debuff
        };
    }

    //플레이어 인벤토리 DTO 구성
    private List<ItemInfoDTO> CreateInventoryDTO(List<string> inventoryIds)
    {
        List<ItemInfoDTO> invList = new List<ItemInfoDTO>();
        foreach (var id in inventoryIds)
        {
            ItemSO itemSo = ItemDB.Instance.Get(id);
            if (itemSo != null)
            {
                invList.Add(new ItemInfoDTO
                {
                    itemId = itemSo.itemId,
                    name = itemSo.displayName_ID,
                    cost = itemSo.itemCost,
                    rarity = itemSo.itemClass,
                    type = itemSo.type
                });
            }
        }
        return invList;
    }

    // private PlayerVStateDTO CreatePlayerVDTO(int curGold, List<VillageObjInfo> villObjInfos)
    // {
    //     //return new
    // }


    //플레이어 인벤토리에 아이템 추가
    public void TryAddItemToInventory(int playerNum, string itemId)
    {
        if (playerNum == 1)
        {
            if (p1Inventory.Count < 8)
            {
                p1Inventory.Add(itemId);
                OnItemAdded?.Invoke(playerNum, itemId);
                PrintPlayerInventory(p1Inventory);
            }
        }
        else if (playerNum == 2)
        {
            if (p2Inventory.Count < 8)
            {
                p2Inventory.Add(itemId);
                OnItemAdded?.Invoke(playerNum, itemId);
                PrintPlayerInventory(p2Inventory);
            }
        }
        else
            Debug.LogWarning($"P{playerNum}'s Inventory Is Full");
    }

    //플레이어 인벤토리에서 아이템 삭제
    public void TryDeleteItemFromInventory(int playerNum, string itemId)
    {
        bool isRemoved;
        if (playerNum == 1)
        {
            isRemoved = p1Inventory.Remove(itemId);
            OnItemRemoved?.Invoke(playerNum, itemId);
            PrintPlayerInventory(p1Inventory);
        }
        else
        {
            isRemoved = p2Inventory.Remove(itemId);
            OnItemRemoved?.Invoke(playerNum, itemId);
            PrintPlayerInventory(p2Inventory);
        }
        if (!isRemoved) Debug.LogWarning($"Faild to delete {itemId} item from P{playerNum}'s Inventory");
    }

    public void ApplyTreeDamage(int playerNum, float hitDamage)
    {
        //아이템 효과가 적용된 데미지 입히기
        //그에 맞는 마을 방어벽 설정

        //임시로 구현된 코드로 수정 필요
        this.treeHP -= hitDamage;
        Debug.Log($"Hit Damage : {hitDamage}, Tree HP : {this.treeHP}");
        if (playerNum == 1)
        {
            p1TotalHitDmg += hitDamage;
            p1VillBarrier += hitDamage * p1VillBarConRate;
            Debug.Log($"p{playerNum} Village Barrier : {p1VillBarrier}");
        }
        else
        {
            p2TotalHitDmg += hitDamage;
            p2VillBarrier += hitDamage * p2VillBarConRate;
            Debug.Log($"p{playerNum} Village Barrier : {p2VillBarrier}");
        }
    }

    public void ApplyToxicToVillage()
    {
        float p1AcutalDamage = Mathf.Max(0, this.treeToxicDmg - this.p1VillBarrier);
        this.p1VillHP -= p1AcutalDamage;

        float p2AcutalDamage = Mathf.Max(0, this.treeToxicDmg - this.p2VillBarrier);
        this.p2VillHP -= p2AcutalDamage;

        this.treeToxicDmg = Mathf.Round(this.roomSetting.treeAtkPow * Mathf.Pow(1.8f, this.day));

        Debug.Log($"p1 Village HP : {this.p1VillHP}, p2 Village HP : {this.p2VillHP}, next tree toxic dmg : {this.treeToxicDmg}");
    }

    public void SetPlayerDmgMultRate(int playerNum, float rate)
    {
        if (playerNum == 1) p1DmgMultRate = rate;
        else p2DmgMultRate = rate;
    }

    public void SetPlayerBarConRate(int playerNum, float rate)
    {
        if (playerNum == 1) p1VillBarConRate = rate;
        else p2VillBarConRate = rate;
    }

    public void InitTurnStat(int playerNum)
    {
        if (playerNum == 1)
        {
            this.p1VillBarConRate = playerSetting.barrierConversionRate;
            this.p1DmgMultRate = 1f;
        }
        else
        {
            this.p2VillBarConRate = playerSetting.barrierConversionRate;
            this.p2DmgMultRate = 1f;
        }
    }

    public void InitPlayerStat()
    {
        //방벽 리셋: playerSetting이 아닌 업그레이드된 마을 방어력 적용
        this.p1VillBarrier = simVillageState.p1BasicVillageBarrier;
        this.p2VillBarrier = simVillageState.p2BasicVillageBarrier;

        //기력 회복: (현재 기력 + 일일 기력 회복량)을 더하되, 최대 기력(MaxEnergy)을 넘지 못하게 제한
        this.p1Energy = Mathf.Min(simVillageState.p1DayEnergy, simVillageState.p1MaxEnergy);
        this.p2Energy = Mathf.Min(simVillageState.p2DayEnergy, simVillageState.p2MaxEnergy);

        // 기타 배율 및 누적치 초기화 (기존과 동일)
        this.p1VillBarConRate = this.p2VillBarConRate = playerSetting.barrierConversionRate;
        this.p1TotalHitDmg = this.p2TotalHitDmg = 0f;
        this.p1DmgMultRate = this.p2DmgMultRate = 1f;
    }

    public void PrintPlayerInventory(List<string> Inv)
    {
        string s = "";
        foreach (string i in Inv)
        {
            s += i + " ";
        }
        Debug.Log("player inv : " + s);
    }

}