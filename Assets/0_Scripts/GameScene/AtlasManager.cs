using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoBehaviour
{
    public static AtlasManager instance;
	private SpriteAtlas itemIconAtlas;
	private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

	private void Awake()
	{
		if(instance == null) instance = this;
		else Destroy(gameObject);

		itemIconAtlas = Resources.Load<SpriteAtlas>("SpriteAtlas/DummyItemIcons");
	}

	public Sprite GetItemSprite(string itemId)
	{
		if(itemIconAtlas == null)
		{
			Debug.LogError("Atlas connected error");
			return null;
		}

		if(_spriteCache.ContainsKey(itemId)) return _spriteCache[itemId];

		Sprite newSprite = itemIconAtlas.GetSprite(itemId);
		if (newSprite == null) Debug.LogError("Fail to load sprite from Atlas");
		if (newSprite != null)
		{
			_spriteCache.Add(itemId, newSprite);
		}
		return newSprite;
	}
}
