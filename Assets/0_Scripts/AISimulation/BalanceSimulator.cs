using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SimGameState
{
    public int wave;
    public int day;

    public int p1Energy, p2Energy;
    public float p1VillHP, p1VillBarrier;
    public float p2VillHP, p2VillBarrier;
    public bool p1HasDebuff, p2HasDebuff;
    public float treeHP;
    public float treeToxicDmg;

    public List<string> p1Inventory = new List<string>();
    public List<string> p2Inventory = new List<string>();

    public SimGameState(PlayerSetting playerSetting, RoomSetting roomSetting)
    {
        this.p1Energy = this.p2Energy = playerSetting.initialEnergy;
        this.p1VillHP = this.p2VillHP = playerSetting.villageHP;
        this.p1VillBarrier = this.p2VillBarrier = playerSetting.villageBarrier;
        this.treeHP = roomSetting.treeHP;
        this.treeToxicDmg = roomSetting.treeAtkPow;
        this.wave = roomSetting.initialWave;
        this.day = roomSetting.startDay;
    }

    public LLMGameStateDTO GetStateForPlayer(int myPlayerNum)
    {
        LLMGameStateDTO dto = new LLMGameStateDTO();

        dto.currentWave = this.wave;
        dto.expectedToxicDamage = this.treeToxicDmg;
        dto.treeHP = this.treeHP;

        if (myPlayerNum == 1)
        {
            dto.myStatus = CreatePlayerDTO(p1VillHP, p1VillBarrier, p1Energy, p1HasDebuff);
            dto.oppStatus = CreatePlayerDTO(p2VillHP, p2VillBarrier, p2Energy, p2HasDebuff);
            dto.myInventory = CreateInventoryDTO(p1Inventory);
            dto.oppInventory = CreateInventoryDTO(p2Inventory);
        }
        else
        {
            dto.myStatus = CreatePlayerDTO(p2VillHP, p2VillBarrier, p2Energy, p2HasDebuff);
            dto.oppStatus = CreatePlayerDTO(p1VillHP, p1VillBarrier, p1Energy, p1HasDebuff);
            dto.myInventory = CreateInventoryDTO(p2Inventory);
            dto.oppInventory = CreateInventoryDTO(p1Inventory);
        }
        return dto;
    }

    private PlayerStatusDTO CreatePlayerDTO(float hp, float barrier, int energy, bool Debuff)
    {
        return new PlayerStatusDTO
        {
            villageHP = hp,
            barrier = barrier,
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

public class BalanceSimulator : MonoBehaviour
{
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    private OllamaAPIClient apiClient = new OllamaAPIClient();
    private string csvPath;

    private int gameCount = 10;

    private void Start()
    {
        csvPath = Application.dataPath + "/BalanceResult.csv";
        File.WriteAllText(csvPath, "GameNum,Winner,TotalTurns,P1_Items,P2_Items\n");

    }

    private async UniTask RunMassiveSimulation()
    {
        for (int cnt = 1; cnt <= gameCount; cnt++)
        {
            SimGameState state = new SimGameState(playerSetting, roomSetting);
            bool isGameOver = false;
            int winner = 0;
            int turnCount = 0;

            while (!isGameOver && turnCount < 50)
            {
                turnCount++;


            }
        }
    }
}
