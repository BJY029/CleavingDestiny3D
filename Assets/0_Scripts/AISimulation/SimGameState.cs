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
    public float p1VillHP, p1VillBarrier;
    public float p2VillHP, p2VillBarrier;
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

}