using TMPro;
using UnityEngine;

namespace Village
{
    public class VillageStatusUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject statusPanel;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI statusText;

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        public void Open()
        {
            Refresh();
            statusPanel.SetActive(true);
            // statusPanel.transform.SetAsLastSibling();
        }

        public void Close()
        {
            statusPanel.SetActive(false);
        }

        public void Refresh()
        {
            IVillageStatProvider stat = VillageSystem.VillageStat;
            int mineLevel = stat.GetVillageLevel(VillageType.Mine);
            int forgeLevel = stat.GetVillageLevel(VillageType.Forge);
            int shopLevel = stat.GetVillageLevel(VillageType.Shop);
            int farmLevel = stat.GetVillageLevel(VillageType.Farm);
            int barrierLevel = stat.GetVillageLevel(VillageType.Barrier);
            var damage = stat.GetAxeRangeDamage(forgeLevel);

            string content = LocalizationManager.Instance.GetFormatText(
                CSV_Type.Village,
                "Status_Content",
                VillageSystem.VillageLogic.GetMyGold(),
                mineLevel + 1,
                stat.GetGoldIncomePerDay(mineLevel),
                forgeLevel + 1,
                damage.min,
                damage.max,
                shopLevel + 1,
                farmLevel + 1,
                stat.GetMaxEnergy(farmLevel),
                stat.GetEnergyIncomePerDay(farmLevel),
                barrierLevel + 1,
                stat.GetBarrierArmor(barrierLevel));
            statusText.richText = true;
            statusText.SetText(content);
        }

        private void OnLanguageChanged()
        {
            if (statusPanel.activeSelf) Refresh();
        }
    }
}
