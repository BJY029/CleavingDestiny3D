using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SimGameState
{
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;
    public int roomSeed;

    public int curTurnPlayerNum;

    public int turn;
    public int wave;
    public int day;

    public float p1TotalHitDmg, p2TotalHitDmg;
    public int p1MaxHitDmg, p2MaxHitDmg;
    public int p1MinHitDmg, p2MinHitDmg;
    public int p1Energy, p2Energy;
    public int p1MaxEnergy, p2MaxEnergy;
    public float p1VillHP, p1VillBarrier, p1VillBarConRate;
    public float p2VillHP, p2VillBarrier, p2VillBarConRate;
    public bool p1HasDebuff, p2HasDebuff;
    public float treeHP;
    public float treeToxicDmg;

    public int p1LockCnt, p2LockCnt;
    public int p1LockpickCnt, p2LockpickCnt;
    public List<string> p1Inventory = new List<string>();
    public List<string> p2Inventory = new List<string>();

    //생성자
    public SimGameState(PlayerSetting playerSetting, RoomSetting roomSetting)
    {
        this.playerSetting = playerSetting;
        this.roomSetting = roomSetting;
        this.p1Energy = this.p1MaxEnergy = this.p2Energy = this.p2MaxEnergy = playerSetting.initialEnergy;
        this.p1TotalHitDmg = this.p2TotalHitDmg = 0f;
        this.p1MaxHitDmg = this.p2MaxEnergy = playerSetting.maxAtkPow;
        this.p1MinHitDmg = this.p2MinHitDmg = playerSetting.minAtkPow;
        this.p1VillHP = this.p2VillHP = playerSetting.villageHP;
        this.p1VillBarrier = this.p2VillBarrier = playerSetting.villageBarrier;
        this.p1VillBarConRate = this.p2VillBarConRate = playerSetting.barrierConversionRate;
        this.treeHP = roomSetting.treeHP;
        this.treeToxicDmg = roomSetting.treeAtkPow;
        this.turn = 0;
        this.wave = roomSetting.initialWave;
        this.day = roomSetting.startDay;

        this.p1LockCnt = this.p2LockCnt = 1;
        this.p1LockpickCnt = this.p2LockpickCnt = 0;
        this.roomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
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
            dto.myStatus = CreatePlayerDTO(p1MaxHitDmg, p1MinHitDmg, p1VillHP, p1VillBarrier, p1MaxEnergy, p1Energy, p1HasDebuff);
            dto.oppStatus = CreatePlayerDTO(p2MaxHitDmg, p2MinHitDmg, p2VillHP, p2VillBarrier, p2MaxEnergy, p2Energy, p2HasDebuff);
            dto.myInventory = CreateInventoryDTO(p1Inventory);
            dto.oppInventory = CreateInventoryDTO(p2Inventory);
        }
        else
        {
            dto.myStatus = CreatePlayerDTO(p2MaxHitDmg, p2MinHitDmg, p2VillHP, p2VillBarrier, p2MaxEnergy, p2Energy, p2HasDebuff);
            dto.oppStatus = CreatePlayerDTO(p1MaxHitDmg, p1MinHitDmg, p1VillHP, p1VillBarrier, p1MaxEnergy, p1Energy, p1HasDebuff);
            dto.myInventory = CreateInventoryDTO(p2Inventory);
            dto.oppInventory = CreateInventoryDTO(p1Inventory);
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

    //플레이어 인벤토리에 아이템 추가
    public void TryAddItemToInventory(int playerNum, string itemId)
    {
        if (playerNum == 1)
        {
            if (p1Inventory.Count < 8)
            {
                p1Inventory.Add(itemId);
                PrintPlayerInventory(p1Inventory);
            }
        }
        else if (playerNum == 2)
        {
            if (p2Inventory.Count < 8)
            {
                p2Inventory.Add(itemId);
                PrintPlayerInventory(p2Inventory);
            }
        }
        else
            Debug.LogWarning($"P{playerNum}'s Inventory Is Full");
    }

    //플레이어 인벤토리에서 아이템 삭제
    public void TryDeleteItemFromInventroy(int playerNum, string itemId)
    {
        bool isRemoved;
        if (playerNum == 1)
        {
            isRemoved = p1Inventory.Remove(itemId);
            PrintPlayerInventory(p1Inventory);
        }
        else
        {
            isRemoved = p2Inventory.Remove(itemId);
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
            Debug.Log($"p{playerNum} Village Barrier : {p1VillBarrier}");
        }
    }

    public void ApplyToxicToVillage()
    {
        this.p1VillHP = this.p1VillHP + this.p1VillBarrier - this.treeToxicDmg;
        this.p2VillHP = this.p2VillHP + this.p2VillBarrier - this.treeToxicDmg;
        this.treeToxicDmg = Mathf.Round(this.roomSetting.treeAtkPow * Mathf.Pow(1.8f, this.day));

        Debug.Log($"p1 Village HP : {this.p1VillHP}, p2 Village HP : {this.p2VillHP}, next tree toxic dmg : {this.treeToxicDmg}");
        InitPlayerStat();
    }

    public void InitPlayerStat()
    {
        this.p1VillBarrier = this.p2VillBarrier = playerSetting.villageBarrier;
        this.p1VillBarConRate = this.p2VillBarConRate = playerSetting.barrierConversionRate;
        this.p1TotalHitDmg = this.p2TotalHitDmg = 0f;
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


    public float GetTreeHP()
    {
        return this.treeHP;
    }

    public void SetTreeHP(float newHP)
    {
        this.treeHP = newHP;
    }

    public float GetPlayerVillageHP(int playerNum)
    {
        return playerNum == 1 ? this.p1VillHP : this.p2VillHP;
    }

    public void SetPlayerVIllageHP(int playerNum, float newHP)
    {
        if (playerNum == 1) p1VillHP = newHP;
        else p2VillHP = newHP;
    }

    public float GetPlayerVillageShield(int playerNum)
    {
        return playerNum == 1 ? this.p1VillBarrier : this.p2VillBarrier;
    }

    public void SetPlayerVIllageShield(int playerNum, float newValue)
    {
        if (playerNum == 1) p1VillBarrier = newValue;
        else p2VillBarrier = newValue;
    }

    public float GetBarrierConversionRate(int playerNum)
    {
        return playerNum == 1 ? this.p1VillBarConRate : this.p2VillBarConRate;
    }

    public void SetBarrierConversionRate(int playerNum, float newValue)
    {
        if (playerNum == 1) p1VillBarConRate = newValue;
        else p2VillBarConRate = newValue;
    }

    public int GetPlayerEng(int playerNum)
    {
        return playerNum == 1 ? this.p1Energy : this.p2Energy;
    }

    public void SetPlayerEng(int playerNum, int newValue)
    {
        if (playerNum == 1) p1Energy = newValue;
        else p2Energy = newValue;
    }

    public int GetPlayerLockpickCount(int playerNum)
    {
        return playerNum == 1 ? this.p1LockpickCnt : this.p2LockpickCnt;
    }

    public void AddPlayerLockPickCount(int playerNum)
    {
        if (playerNum == 1) p1LockpickCnt++;
        else p2LockpickCnt++;
    }

    public void RemovePlayerLockPickCount(int playerNum)
    {
        if (playerNum == 1) p1LockpickCnt = Mathf.Max(p1LockpickCnt - 1, 0);
        else p2LockpickCnt = Mathf.Max(p2LockpickCnt - 1, 0);
    }

    public void AddPlayerLockCount(int playerNum)
    {
        if (playerNum == 1) p1LockCnt++;
        else p2LockCnt++;
    }


}