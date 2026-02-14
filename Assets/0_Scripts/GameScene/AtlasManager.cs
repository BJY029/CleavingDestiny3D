using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AtlasManager : MonoBehaviour
{
	public static AtlasManager instance;
	private SpriteAtlas itemIconAtlas;
	private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);

		Addressables.LoadAssetAsync<SpriteAtlas>("DummyItemIcons").Completed += OnAtlasLoaded;
	}

	private void OnAtlasLoaded(AsyncOperationHandle<SpriteAtlas> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			itemIconAtlas = handle.Result;
		}
		else
		{
			Debug.LogError("아틀라스 로드 실패");
		}
	}

	public Sprite GetItemSprite(string itemId)
	{
		if (itemIconAtlas == null)
		{
			Debug.LogError("Atlas connected error");
			return null;
		}

		if (_spriteCache.ContainsKey(itemId)) return _spriteCache[itemId];

		Sprite newSprite = itemIconAtlas.GetSprite(itemId);
		if (newSprite == null) Debug.LogError("Fail to load sprite from Atlas");
		if (newSprite != null)
		{
			_spriteCache.Add(itemId, newSprite);
		}
		return newSprite;
	}
}
