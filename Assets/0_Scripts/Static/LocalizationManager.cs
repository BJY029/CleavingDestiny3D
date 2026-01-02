using System;
using System.Collections.Generic;
using System.Text.RegularExpressions; // 정규표현식 사용을 위해 추가
using UnityEngine;

public enum Language
{
	KR,
	EN,
}

public enum CSV_Type
{
	UI, Item
}

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager Instance;

	[Header("CSV Files")]
	public TextAsset UI_CSV;
	public TextAsset Item_CSV;

	[Header("Settings")]
	public Language currentLanguage = Language.KR;

	// 딕셔너리 관리 (데이터 무결성을 위해 private 유지)
	private Dictionary<string, string> _UITable = new Dictionary<string, string>();
	private Dictionary<string, string> _ItemTable = new Dictionary<string, string>();

	public event Action OnLanguageChanged;

	// CSV 파싱용 정규표현식 (따옴표 안의 쉼표는 무시하고 분리)
	private readonly string CSV_SPLIT_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

	private void Awake()
	{
		// 1. 싱글턴 로직 수정: 중복된 '새로운' 객체를 파괴해야 함
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);

		LoadAllData();
	}

	private void LoadAllData()
	{
		LoadCSV(UI_CSV, _UITable);
		LoadCSV(Item_CSV, _ItemTable);
	}

	private void LoadCSV(TextAsset ta, Dictionary<string, string> dic)
	{
		dic.Clear();

		if (ta == null)
		{
			Debug.LogError($"[Localization] CSV File is missing.");
			return;
		}

		// 윈도우(\r\n), 맥/리눅스(\n) 줄바꿈 모두 대응
		string[] lines = ta.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		if (lines.Length <= 1) return;

		// 헤더 파싱
		string[] header = lines[0].Trim().Split(',');

		// 현재 언어 컬럼 찾기
		string langColumnName = currentLanguage.ToString();
		int langIndex = Array.IndexOf(header, langColumnName);
		int idIndex = Array.IndexOf(header, "ID");

		if (langIndex == -1 || idIndex == -1)
		{
			Debug.LogError($"[Localization] Header Error in {ta.name}. ID or {langColumnName} column not found.");
			return;
		}

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (string.IsNullOrWhiteSpace(line)) continue;

			// 2. 단순 Split(',') 대신 정규표현식 사용 (따옴표 안 쉼표 보호)
			string[] cols = Regex.Split(line, CSV_SPLIT_REGEX);

			if (cols.Length <= Mathf.Max(idIndex, langIndex)) continue;

			// 3. 데이터 정제 (따옴표 제거 및 줄바꿈 처리)
			string id = Unquote(cols[idIndex].Trim());
			string text = Unquote(cols[langIndex].Trim());

			// 4. 텍스트 내부의 줄바꿈 기호(\n)를 실제 줄바꿈으로 변환
			text = text.Replace("\\n", "\n");

			if (string.IsNullOrEmpty(id)) continue;

			if (dic.ContainsKey(id))
			{
				Debug.LogWarning($"[Localization] Duplicate ID '{id}' in {ta.name} (line {i + 1})");
				continue;
			}

			dic.Add(id, text);
		}

		Debug.Log($"[Localization] Loaded {ta.name} : {dic.Count} entries.");
	}

	// 엑셀 CSV 특유의 따옴표 처리 제거 함수
	// 예: "Hello, World" -> Hello, World
	private string Unquote(string str)
	{
		if (string.IsNullOrEmpty(str)) return str;

		// 앞뒤가 따옴표로 감싸져 있다면 제거
		if (str.StartsWith("\"") && str.EndsWith("\""))
		{
			str = str.Substring(1, str.Length - 2);
			// 엑셀은 따옴표를 표현할 때 "" 두개를 씀. 이를 하나로 치환
			str = str.Replace("\"\"", "\"");
		}
		return str;
	}

	public string GetText(CSV_Type type, string id)
	{
		if (string.IsNullOrEmpty(id)) return "";

		string result = id; // 기본값은 ID

		switch (type)
		{
			case CSV_Type.UI:
				if (_UITable.TryGetValue(id, out var uiVal)) result = uiVal;
				break;
			case CSV_Type.Item:
				if (_ItemTable.TryGetValue(id, out var itemVal)) result = itemVal;
				break;
		}

		// 값을 못 찾았을 때의 처리 (개발 중에만 경고 로그 추천)
		if (result == id)
		{
			// Debug.LogWarning($"[Localization] Missing Key: {id}");
		}

		return result;
	}

	public void SetLanguage(Language lang)
	{
		if (currentLanguage == lang) return;

		currentLanguage = lang;
		LoadAllData(); // 언어 변경 시 다시 로드
		OnLanguageChanged?.Invoke();
	}
}