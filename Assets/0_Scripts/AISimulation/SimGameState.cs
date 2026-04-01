using UnityEngine;
using System.Collections.Generic;

public class SimGameState
{
    public int roomSeed;

    public int turn;
    public int wave;
    public int day;

    public int p1MaxHitDmg, p2MaxHitDmg;
    public int p1MinHitDmg, p2MinHitDmg;
    public int p1Energy, p2Energy;
    public int p1MaxEnergy, p2MaxEnergy;
    public float p1VillHP, p1VillBarrier, p1VillBarConRate;
    public float p2VillHP, p2VillBarrier, p2VillBarConRate;
    public bool p1HasDebuff, p2HasDebuff;
    public float treeHP;
    public float treeToxicDmg;

    public List<string> p1Inventory = new List<string>();
    public List<string> p2Inventory = new List<string>();

    public SimGameState(PlayerSetting playerSetting, RoomSetting roomSetting)
    {
        this.p1Energy = this.p1MaxEnergy = this.p2Energy = this.p2MaxEnergy = playerSetting.initialEnergy;
        this.p1MaxHitDmg = this.p2MaxEnergy = playerSetting.maxAtkPow;
        this.p1MinHitDmg = this.p2MinHitDmg = playerSetting.minAtkPow;
        this.p1VillHP = this.p2VillHP = playerSetting.villageHP;
        this.p1VillBarrier = this.p2VillBarrier = playerSetting.villageBarrier;
        this.p1VillBarConRate = this.p2VillBarConRate = playerSetting.barrierConversionRate;
        this.treeHP = roomSetting.treeHP;
        this.treeToxicDmg = roomSetting.treeAtkPow;
        this.turn = roomSetting.initialTurnIndex;
        this.wave = roomSetting.initialWave;
        this.day = roomSetting.startDay;
        this.roomSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

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

    public void TryAddItemToInventory(int playerNum, string itemId)
    {
        if (playerNum == 1)
        {
            if (p1Inventory.Count < 8)
                p1Inventory.Add(itemId);
        }
        else
        {
            if (p2Inventory.Count < 8)
                p2Inventory.Add(itemId);
        }
        Debug.LogWarning($"P{playerNum}'s Inventory Is Full");
    }

    public void TryDeleteItemFromInventroy(int playerNum, string itemId)
    {
        bool isRemoved;
        if (playerNum == 1)
            isRemoved = p1Inventory.Remove(itemId);
        else
            isRemoved = p2Inventory.Remove(itemId);
        if (!isRemoved) Debug.LogWarning($"Faild to delete {itemId} item from P{playerNum}'s Inventory");
    }

    public void ApplyTreeDamage(int playerNum, float hitDamage)
    {
        //아이템 효과가 적용된 데미지 입히기
        //그에 맞는 마을 방어벽 설정

        //임시로 구현된 코드로 수정 필요
        this.treeHP -= hitDamage;
        if (playerNum == 1)
        {
            p1VillBarrier += hitDamage * p1VillBarConRate;
        }
        else
        {
            p2VillBarrier += hitDamage * p2VillBarConRate;
        }
    }
}