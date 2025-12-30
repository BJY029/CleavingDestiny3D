using UnityEngine;
using UnityEngine.UI;

public class TreeCanvasController : MonoBehaviour
{
	public static TreeCanvasController Instance;
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public Text TreeHP;

	public void UpdateTreeHP(float damage)
	{
		TreeHP.text = damage.ToString();
	}
}
