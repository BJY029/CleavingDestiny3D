using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Outside
{
    public class VillageHpBar : MonoBehaviour
    {
        /*
            1. 체력, 쉴드, 독성 3개의 수치가 있음
            2. 체력 -> 쉴드 순서대로 배치
            3. 독성은 현재 내구도 오른쪽 끝부터 왼쪽으로 덮어씌움
            4. 독성 데미지 바는 깜빡거리게
            5. 체력이 최대치일 경우 쉴드가 체력을 밀어내며 오른쪽에 붙음
            6. 체력이 감소한 상태일 경우 감소한 빈칸에 쉴드를 채우며 감소한 수치보다 쉴드가 많을 경우 5번처럼 체력을 밀어냄
            7. 스탯 표시하기

            - 체력, 쉴드, 독 이미지는 전부 상하 stretch 모드임
        */

        public RectTransform hpBarFullRect;

        public Image hpBar;
        public Image shieldBar;
        public Image poisonBar;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI shieldText;
        public TextMeshProUGUI poisonText;

        const string HP_STRING = "{0}/{1}";
        const string SHIELD_STRING = "Shield: {0}";
        const string POISON_STRING = "Poison: {0}";

        Tween _poisonTween;

        private void Start()
        {
            // 4. 독성 데미지 바는 깜빡거리게
            if (poisonBar != null)
            {
                _poisonTween = Tween.Alpha(poisonBar, 0.2f, 0.8f, 0.5f, Ease.Linear, -1, CycleMode.Yoyo);
            }
        }

        public void UpdateStats(float currentHp, float maxHp, float shield, float poison)
        {
            // 7. 스탯 표시하기
            hpText.SetText(HP_STRING, currentHp, maxHp);
            shieldText.SetText(SHIELD_STRING, shield);
            poisonText.SetText(POISON_STRING, poison);

            if (maxHp <= 0) return;

            float totalWidth = hpBarFullRect.rect.width;

            // 비율 기준 계산
            // 기본은 MaxHp, 하지만 체력+방벽 합이 MaxHp를 넘으면 그 합을 기준으로 삼음
            // (넘치지 않으면 MaxHp가 전체 길이, 넘치면 합계가 전체 길이)
            float currentTotalWithShield = currentHp + shield;
            float renderCapacity = Mathf.Max(maxHp, currentTotalWithShield);

            // 0으로 나누기 방지
            if (renderCapacity <= 0) renderCapacity = 1;

            // 1. 체력 Width 계산
            float clampedHp = Mathf.Clamp(currentHp, 0, maxHp);
            float hpRatio = clampedHp / renderCapacity;
            float hpWidth = totalWidth * hpRatio;

            // 2. 방벽 Width 계산
            float shieldRatio = shield / renderCapacity;
            float shieldWidth = totalWidth * shieldRatio;

            // Constraint: 체력 + 방벽 너비는 전체 너비를 초과할 수 없음 (부동소수점 오차 보정)
            // renderCapacity 로직상 이론적으로는 넘지 않지만, 확실하게 clamping
            if (hpWidth + shieldWidth > totalWidth)
            {
                shieldWidth = totalWidth - hpWidth;
            }
            if (shieldWidth < 0) shieldWidth = 0;

            // 3. 독성(데미지) Width 계산
            float poisonRatio = poison / renderCapacity;
            float poisonWidth = totalWidth * poisonRatio;

            // Constraint: 독성 바의 길이는 현재 보이는 바(체력+방벽)의 총 길이를 넘어서면 안됨 
            float currentVisibleBarWidth = hpWidth + shieldWidth;
            if (poisonWidth > currentVisibleBarWidth)
            {
                poisonWidth = currentVisibleBarWidth;
            }

            // 체력과 방벽은 왼쪽부터, 들어올 피해는 현재 총 내구도의 오른쪽부터 표시
            SetBar(hpBar.rectTransform, 0f, hpWidth);
            SetBar(shieldBar.rectTransform, hpWidth, shieldWidth);
            SetBar(poisonBar.rectTransform, hpWidth + shieldWidth, poisonWidth);

            // Visibility
            shieldBar.gameObject.SetActive(shieldWidth > 0);
            poisonBar.gameObject.SetActive(poisonWidth > 0);
        }

        private void SetBar(RectTransform rt, float x, float width)
        {
            Vector2 position = rt.anchoredPosition;
            position.x = x;
            rt.anchoredPosition = position;

            Vector2 size = rt.sizeDelta;
            size.x = width;
            rt.sizeDelta = size;
        }
    }
}
