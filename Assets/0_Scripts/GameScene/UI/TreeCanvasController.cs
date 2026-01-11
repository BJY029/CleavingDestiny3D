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

	public void UpdateTreeHP(float damage)
	{
		TreeHP.text = damage.ToString();
	}
}
