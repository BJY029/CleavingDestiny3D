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

	public void UpdateTreeHP(float maxTreeHP, float curTreeHP)
	{
		// Room properties may not have arrived when the master initializes the UI.
		// A NaN slider value can leave its fill transform corrupted even after HP arrives.
		if (maxTreeHP <= 0f || float.IsNaN(maxTreeHP) || float.IsInfinity(maxTreeHP)
			|| float.IsNaN(curTreeHP) || float.IsInfinity(curTreeHP)) return;

		TreeHPSlider.value = curTreeHP / maxTreeHP;
		TreeHP.text = curTreeHP.ToString();
	}
}
