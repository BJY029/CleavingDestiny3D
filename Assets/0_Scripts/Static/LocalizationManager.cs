using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public enum Language { KR, EN }
public enum CSV_Type { UI, Item, Preload, Village }

public class LocalizationManager : MonoBehaviour
{
	private static LocalizationManager _instance;
	public static LocalizationManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<LocalizationManager>();
				// 씬에 없으면 새로 생성 (이 시스템으로 인해 인스펙터로 설정이 불가능한 점 유의)
				if (_instance == null)
				{
					_instance = new GameObject("LocalizationManager").AddComponent<LocalizationManager>();
				}
			}
			return _instance;
		}
	}

	[Header("Settings")]
	public Language currentLanguage = Language.KR;

	private const string PreloadCSVPath = "Preload_CSV";
	private const string LanguageLabel = "Localization";

	private Dictionary<CSV_Type, Dictionary<string, string>> _tables = new Dictionary<CSV_Type, Dictionary<string, string>>();

	public event Action OnLanguageChanged;
	public bool IsLoaded { get; private set; } = false;

	private readonly string CSV_SPLIT_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad(gameObject);

		InitializeLocalization().Forget();
	}

	private async UniTaskVoid InitializeLocalization()
	{
		IsLoaded = false;
		_tables.Clear();
		foreach (CSV_Type type in Enum.GetValues(typeof(CSV_Type)))
			_tables[type] = new Dictionary<string, string>();

		// 1. Preload_CSV 동기 로드
		LoadPreloadData();

		// 2. Addressables 비동기 로드
		await LoadAddressableData();

		IsLoaded = true;
		OnLanguageChanged?.Invoke();
	}

	private void LoadPreloadData()
	{
		TextAsset ta = Resources.Load<TextAsset>(PreloadCSVPath);
		if (ta != null)
		{
			ParseCSV(ta.text, _tables[CSV_Type.Preload]);
			Debug.Log("[Localization] Preload Data Loaded.");
		}
	}

	private async UniTask LoadAddressableData()
	{
		AsyncOperationHandle<IList<TextAsset>> handle = Addressables.LoadAssetsAsync<TextAsset>(LanguageLabel, null);

		try
		{
			var result = await handle;
			var enumList = Enum.GetValues(typeof(CSV_Type));

			foreach (var ta in result)
			{
				CSV_Type assignedType = CSV_Type.UI; // 기본값

				// Enum 이름을 순회하며 파일명 시작 부분 확인
				foreach (CSV_Type type in enumList)
				{
					if (ta.name.StartsWith(type.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						assignedType = type;
						break;
					}
				}

				ParseCSV(ta.text, _tables[assignedType]);
			}
			Debug.Log("[Localization] Addressable Assets Loaded.");
		}
		finally
		{
			if (handle.IsValid())
				Addressables.Release(handle);
		}
	}

	public void LoadAllData() => InitializeLocalization().Forget();

	private void ParseCSV(string csvText, Dictionary<string, string> dic)
	{
		using StringReader reader = new StringReader(csvText);
		string line = reader.ReadLine();
		if (line == null) return;

		string[] header = Regex.Split(line, CSV_SPLIT_REGEX);
		int idIndex = Array.IndexOf(header, "ID");
		int langIndex = Array.IndexOf(header, currentLanguage.ToString());

		if (idIndex == -1 || langIndex == -1) return;

		while ((line = reader.ReadLine()) != null)
		{
			if (string.IsNullOrWhiteSpace(line)) continue;

			string[] cols = Regex.Split(line, CSV_SPLIT_REGEX);
			if (cols.Length <= Math.Max(idIndex, langIndex)) continue;

			string id = Unquote(cols[idIndex].Trim());
			string text = Unquote(cols[langIndex].Trim()).Replace("\\n", "\n");

			if (!string.IsNullOrEmpty(id) && !dic.ContainsKey(id))
			{
				dic.Add(id, text);
			}
		}
	}

	private string Unquote(string str)
	{
		if (string.IsNullOrEmpty(str)) return str;
		str = str.Trim();
		if (str.StartsWith("\"") && str.EndsWith("\""))
		{
			str = str.Substring(1, str.Length - 2);
			str = str.Replace("\"\"", "\"");
		}
		return str;
	}

	public string GetText(CSV_Type type, string id)
	{
		if (_tables.TryGetValue(type, out var table))
		{
			if (table.TryGetValue(id, out var value))
				return value;
		}
		return id;
	}

	public void SetLanguage(Language lang)
	{
		if (currentLanguage == lang) return;
		currentLanguage = lang;
		InitializeLocalization().Forget();
	}
}