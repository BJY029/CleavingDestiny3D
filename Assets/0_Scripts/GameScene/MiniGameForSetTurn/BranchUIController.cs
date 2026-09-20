using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BranchUIController : MonoBehaviour
{
	public static BranchUIController Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	public Canvas BranchCanvas;
	public TextMeshProUGUI Desc1;
	public TextMeshProUGUI Desc2;
	public GameObject WoodCurtain;

	private float offDuration = 2f;
	private float waitDuration = 2f;

	private void Start()
	{
		Desc1.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_BranchDesc1);
		Desc2.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_BranchDesc2);
	}


	public IEnumerator FadeoutCurtain_GameStart()
	{
		float time = 0f;

		while (time < offDuration)
		{
			time += Time.deltaTime;
			float moveAmount = 6f * Time.deltaTime;

			WoodCurtain.transform.position += Vector3.back * moveAmount;

			yield return null;
		}

		//yield return new WaitForSeconds(waitDuration);
		BranchCanvas.gameObject.SetActive(false);
		Destroy(WoodCurtain);
	}
}
