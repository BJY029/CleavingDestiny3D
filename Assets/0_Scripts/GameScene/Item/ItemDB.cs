using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemDB : MonoBehaviour
{
	//싱글턴
	public static ItemDB Instance;
	//스크립터블 오브젝트 아이템 리스트
	[SerializeField] List<ItemSO> items;
	//아이템 DB
	Dictionary<string, ItemSO> map;
	[SerializeField] Dictionary<string, Material> itemMat;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

	}

	private void Start()
	{
		//아이템 DB 초기화
		map = new Dictionary<string, ItemSO>();
		foreach (var it in items)
		{
			if (!string.IsNullOrEmpty(it.itemId))
				map[it.itemId] = it;
		}

		itemMat = new Dictionary<string, Material>();

		Addressables.LoadAssetsAsync<Material>("ItemMaterial", null).Completed += OnAllMaterialLoaded;

	}

	private void OnAllMaterialLoaded(AsyncOperationHandle<IList<Material>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (var mat in handle.Result)
			{
				if (!itemMat.ContainsKey(mat.name))
				{
					itemMat.Add(mat.name, mat);
				}
			}
			Debug.Log($"머티리얼 로드 완료! 총 개수: {itemMat.Count}개");
		}
		else
		{
			Debug.LogError("머티리얼 로드 실패. 'ItemMaterial' 라벨이 정확히 붙어있는지 확인해주세요.");
		}
	}


	//아이템 객체 얻기
	public ItemSO Get(string id)
	{
		return (id != null && map.TryGetValue(id, out var it)) ? it : null;
	}

	public Material GetMat(string id)
	{
		return (id != null && itemMat.TryGetValue(id, out var it)) ? it : null;
	}

	public List<ItemSO> GetItemsList()
	{
		return items;
	}
}
