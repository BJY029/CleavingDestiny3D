using System;
using System.Collections.Generic;
using UnityEngine;

//언어 Enum
public enum Language
{
    KR,
    EN,
}

public class LocalizationManager : MonoBehaviour
{
	//싱글턴
    public static LocalizationManager Instance;

	//UI 관련 CSV 파일
	public TextAsset UI_CSV;
	//현재 설정된 언어
	public Language currentLanguage = Language.KR;
	//CSV 파일을 저장할 딕셔너리
	private Dictionary<string, string> _UITable = new Dictionary<string, string>();
	//언어 변경시 Invoke 될 액션
	public event Action OnLanguageChanged;

	private void Awake()
	{
		//싱글턴 처리
		if(Instance != null && Instance != this)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(Instance);

		LoadCSV();
	}

	private void LoadCSV()
	{
		_UITable.Clear();

		//예외 처리
		if(UI_CSV == null)
		{
			Debug.LogError("Null UI CSV File");
			return;
		}

		//각 줄을 lines에 저장
		string[] lines = UI_CSV.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		//헤더만 있고 값이 없는 경우
		if(lines.Length <= 1)
		{
			Debug.LogError("No Content in CSV File");
			return;
		}

		//헤더 정보 저장
		string[] header = lines[0].Trim().Split(',');

		//설정된 언어에 맞는 헤더 이름 가져오기
		string langColumnName = currentLanguage.ToString();
		//언어에 맞는 열 인덱스 가져오기
		int langIndex = Array.IndexOf(header, langColumnName);

		if (langIndex == -1)
		{
			Debug.LogError("No Language Name Exist");
			return;
		}

		int idIndex = Array.IndexOf(header, "ID");

		if(idIndex == -1)
		{
			Debug.LogError("No ID Exist");
			return;
		}

		//각 줄을 돌면서
		for(int i = 1; i < lines.Length; i++)
		{
			//양쪽 끝 공백 없애고
			string line = lines[i].Trim();
			//예외 처리
			if (string.IsNullOrWhiteSpace(line))
				continue;

			//각 문장을 쉼표로 나눈 후
			string[] cols = line.Split(",");
			//Id 및 맞는 언어 인덱스 값이 실제 값 범위 내에 있는지 확인
			if (cols.Length <= Mathf.Max(idIndex, langIndex))
				continue;

			//각 id와 매핑되는 텍스트 가져오기
			string id = cols[idIndex].Trim();
			string text = cols[langIndex].Trim();

			if (string.IsNullOrEmpty(id))
				continue;

			if(_UITable.ContainsKey(id))
			{
				Debug.LogWarning($"Duplicate ID detected : {id} (line {i+1})");
				continue;
			}

			//딕셔너리 삽입
			_UITable.Add(id, text);
		}

		Debug.Log($"Complited UI CSV Load. Component count = {_UITable.Count}");
	}

	//id 기반 텍스트 반환
	//만약 해당되는 id가 없으면 id 그대로 반환
	public string GetText(string id)
	{
		if(string.IsNullOrEmpty(id))
			return string.Empty;

		if(_UITable.TryGetValue(id, out var value))
			return value;

		Debug.LogWarning("$No Found : {id}");
		return id;
	}

	//언어 관련 설정
	public void SetLanguage(Language lang)
	{
		if(currentLanguage == lang) return;

		currentLanguage = lang;
		LoadCSV();
		OnLanguageChanged?.Invoke();
	}
}
