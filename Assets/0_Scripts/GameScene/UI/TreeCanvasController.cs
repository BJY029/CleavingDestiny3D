using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreeCanvasController : MonoBehaviour
{
	public static TreeCanvasController Instance;
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public TextMeshProUGUI TreeHP;
	public Slider TreeHPSlider;
	[SerializeField] private float hiddenAlpha = 0.35f;

	private CanvasGroup sliderCanvasGroup;
	private bool isTreeHPHidden;

	private void EnsureSliderCanvasGroup()
	{
		if (sliderCanvasGroup != null || TreeHPSlider == null) return;
		if (!TreeHPSlider.TryGetComponent(out sliderCanvasGroup))
			sliderCanvasGroup = TreeHPSlider.gameObject.AddComponent<CanvasGroup>();
	}

	public void SetTreeHPHidden(bool hidden)
	{
		isTreeHPHidden = hidden;

		if (TreeHPSlider != null)
		{
			EnsureSliderCanvasGroup();
			if (sliderCanvasGroup != null)
				sliderCanvasGroup.alpha = hidden ? hiddenAlpha : 1f;

			if (TreeHPSlider.fillRect != null)
				TreeHPSlider.fillRect.gameObject.SetActive(!hidden);
		}

		if (!hidden && TreeStatus.Instance != null)
			TreeStatus.Instance.SetTreeStatusUI();
	}

	public void UpdateTreeHP(float maxTreeHP, float curTreeHP)
	{
		if (isTreeHPHidden) return;

		// Room properties may not have arrived when the master initializes the UI.
		// A NaN slider value can leave its fill transform corrupted even after HP arrives.
		if (maxTreeHP <= 0f || float.IsNaN(maxTreeHP) || float.IsInfinity(maxTreeHP)
			|| float.IsNaN(curTreeHP) || float.IsInfinity(curTreeHP)) return;

		TreeHPSlider.value = curTreeHP / maxTreeHP;
		TreeHP.text = curTreeHP.ToString();
	}
}
