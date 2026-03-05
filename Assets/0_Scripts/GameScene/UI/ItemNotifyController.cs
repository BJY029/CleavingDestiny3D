using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;

public class ItemNotifyController : MonoBehaviour
{
	public TextMeshProUGUI NickName;
	public TextMeshProUGUI Title;
	public TextMeshProUGUI ItemName;
	public TextMeshProUGUI ItemDescription;
	public TextMeshProUGUI ItemRarity;
	public TextMeshProUGUI ItemCost;
	public Image ItemSprite;
	public Animator NotifyAnim;

	public void SetUI(ItemSO item, Player player)
	{
		string name = string.IsNullOrEmpty(player.NickName) ? $"Player{player.ActorNumber}" : player.NickName;

		NickName.text = name + "\'s";
		ItemSprite.sprite = AtlasManager.instance.GetItemSprite(item.itemId);
		Title.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemNotify_Title);
		ItemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
		ItemDescription.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
		ItemRarity.text = item.itemClass.ToString();
		ItemCost.text = "Cost : " + item.itemCost.ToString();
	}

	//Overload
	public void SetUI(ItemSO item, Player FromPlayer, Player ToPlayer)
	{
		string FromName, ToName;

		if ((FromPlayer == null || ToPlayer == null) && GameManager.Instance.isSoloPlay)
		{
			if (FromPlayer == null)
			{
				FromName = "AI";
				ToName = string.IsNullOrEmpty(ToPlayer.NickName) ? $"Player{ToPlayer.ActorNumber}" : ToPlayer.NickName;
			}
			else
			{
				FromName = string.IsNullOrEmpty(FromPlayer.NickName) ? $"Player{FromPlayer.ActorNumber}" : FromPlayer.NickName;
				ToName = "AI";
			}
		}
		else
		{
			FromName = string.IsNullOrEmpty(FromPlayer.NickName) ? $"Player{FromPlayer.ActorNumber}" : FromPlayer.NickName;
			ToName = string.IsNullOrEmpty(ToPlayer.NickName) ? $"Player{ToPlayer.ActorNumber}" : ToPlayer.NickName;
		}

		NickName.text = ToName + " stole " + FromName + "'s item!";
		ItemSprite.sprite = AtlasManager.instance.GetItemSprite(item.itemId);
		Title.text = "";
		ItemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
		ItemDescription.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
		ItemRarity.text = item.itemClass.ToString();
		ItemCost.text = "Cost : " + item.itemCost.ToString();
	}

	public void SetActive(ItemSO item, Player player)
	{
		SetUI(item, player);
		NotifyAnim.Play("UI_Player_ItemNotify_Up");
	}

	public void SetStolenActive(ItemSO item, Player FromPlayer, Player ToPlayer)
	{
		SetUI(item, FromPlayer, ToPlayer);
		NotifyAnim.Play("UI_Player_ItemNotify_Up");
	}
}
