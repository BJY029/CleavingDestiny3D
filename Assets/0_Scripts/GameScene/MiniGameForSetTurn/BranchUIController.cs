using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public Text Desc1;
	public Text Desc2;
	public Image Curtain;

	private float offDuration = 3f;
	private float waitDuration = 2f;

	private void Start()
	{
		Desc1.text = LocalizationManager.Instance.GetText(CSV_Type.UI, "UI_BranchDesc_1");
		Desc2.text = LocalizationManager.Instance.GetText(CSV_Type.UI, "UI_BranchDesc_2");
	}


	public IEnumerator FadeoutCurtain_GameStart()
	{
		Color c = Curtain.color;
		float startAlpha = c.a;
		float time = 0f;

		while(time < offDuration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, 0f, time / offDuration);

			Curtain.color = new Color(c.r, c.g, c.b, alpha);
			yield return null;
		}

		Curtain.color = new Color(c.r, c.g, c.b, 0f);

		yield return new WaitForSeconds(waitDuration);
		BranchCanvas.gameObject.SetActive(false);
	}
}
