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
		TreeHPSlider.value = curTreeHP / maxTreeHP;
		TreeHP.text = curTreeHP.ToString();
	}
}
