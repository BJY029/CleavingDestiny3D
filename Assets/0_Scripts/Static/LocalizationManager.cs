using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using Unity.InferenceEngine.Tokenization.PostProcessors.Templating;

public enum Language { KR, EN }
public enum CSV_Type { UI, Item, Preload, Village, Option, Mission, GuideBook, Shop }

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

	const string languageSaveKey = "Language";

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);

		// 저장된 언어 설정이 있다면 불러오기
		if (PlayerPrefs.HasKey(languageSaveKey))
		{
			string savedLang = PlayerPrefs.GetString(languageSaveKey);
			if (Enum.TryParse(savedLang, out Language lang))
			{
				currentLanguage = lang;
			}
		}

		InitializeLocalization().Forget();
	}

	private async UniTaskVoid InitializeLocalization()
	{
		IsLoaded = false;
		_tables.Clear();
		foreach (CSV_Type type in Enum.GetValues(typeof(CSV_Type)))
			_tables[type] = new Dictionary<string, string>();

		// Preload_CSV 바로 로드
		LoadPreloadData();

		// 나머지 비동기 로드
		await LoadAddressableData();

		IsLoaded = true;
		OnLanguageChanged?.Invoke();
	}

	private void LoadPreloadData()
	{
		// Resources에서 Preload_CSV를 로드하여 UI 테이블에 추가 (메인메뉴 UI 용이라서 UI 테이블에 넣음)
		TextAsset ta = Resources.Load<TextAsset>(PreloadCSVPath);
		if (ta != null)
		{
			var parsedData = ParseCSV(ta.text);
			foreach (var kv in parsedData)
			{
				_tables[CSV_Type.Preload].TryAdd(kv.Key, kv.Value);
			}
			Debug.Log("[Localization] Preload Data Loaded.");
		}
	}

	private async UniTask LoadAddressableData()
	{
		// "Localization" 라벨이 붙은 모든 TextAsset을 로드
		AsyncOperationHandle<IList<TextAsset>> handle = Addressables.LoadAssetsAsync<TextAsset>(LanguageLabel, null);

		try
		{
			// 텍스트 로드 대기
			var result = await handle;
			// 각 CSV 타입에 대하여
			var enumList = Enum.GetValues(typeof(CSV_Type));
			var tasks = new List<UniTask<(CSV_Type, Dictionary<string, string>)>>();

			// 로드된 각 TextAsset을 병렬로 파싱하기 위해 작업 생성
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

				// 멀티스레딩으로 CSV 파싱
				string text = ta.text;
				tasks.Add(UniTask.RunOnThreadPool(() => (assignedType, ParseCSV(text))));
			}

			// 모든 CSV 파싱 작업이 완료될 때까지 대기
			var parsedResults = await UniTask.WhenAll(tasks);

			// 파싱된 결과를 메인 스레드에서 테이블에 통합
			foreach (var (type, data) in parsedResults)
			{
				var table = _tables[type];
				foreach (var kv in data)
				{
					table.TryAdd(kv.Key, kv.Value);
				}
			}

			Debug.Log("[Localization] Addressable Assets Loaded.");
		}
		finally
		{
			// 핸들 해제 (메모리 누수 방지)
			if (handle.IsValid())
				Addressables.Release(handle);
		}
	}

	// 외부에서 모든 데이터를 강제로 다시 로드
	public void LoadAllData() => InitializeLocalization().Forget();

	/// <summary>
	/// CSV 텍스트를 파싱하여 키/값 딕셔너리로 반환합니다.
	/// </summary>
	/// <param name="csvText">파싱할 CSV 원본 텍스트</param>
	/// <returns>ID를 Key로, 현재 언어의 텍스트를 Value로 가지는 Dictionary</returns>
	private Dictionary<string, string> ParseCSV(string csvText)
	{
		var dic = new Dictionary<string, string>();
		if (string.IsNullOrEmpty(csvText)) return dic;

		// CSV 텍스트 전체를 Span으로 참조
		ReadOnlySpan<char> span = csvText.AsSpan();

		// 1. 헤더 행 읽기
		if (!TryReadLine(ref span, out ReadOnlySpan<char> headerLine)) return dic;

		// 헤더 행을 파싱하여 ID 컬럼과 현재 언어 컬럼의 인덱스를 찾습니다.
		var headers = new List<string>();
		SplitCsvLineToStrings(headerLine, headers);

		int idIndex = -1;
		int langIndex = -1;
		string langKey = currentLanguage.ToString();

		for (int i = 0; i < headers.Count; i++)
		{
			string h = headers[i];
			if (string.Equals(h, "ID", StringComparison.OrdinalIgnoreCase)) idIndex = i;
			else if (string.Equals(h, langKey, StringComparison.OrdinalIgnoreCase)) langIndex = i;
		}

		if (idIndex == -1 || langIndex == -1) return dic;

		// 2. 데이터 행 처리
		// 각 행의 컬럼 위치(Range)를 담아둘 리스트
		var columnRanges = new List<Range>();

		while (TryReadLine(ref span, out ReadOnlySpan<char> line))
		{
			// 공백으로만 이루어진 빈 줄 건너뛰기
			if (IsWhiteSpaceSpan(line)) continue;

			// 현재 행을 파싱하여 컬럼들의 Range 정보를 얻음
			columnRanges.Clear();
			SplitCsvLineToRanges(line, columnRanges);

			// 필요한 컬럼이 존재하지 않으면 패스
			if (columnRanges.Count <= Math.Max(idIndex, langIndex)) continue;

			// Range 정보를 이용해 원본 Span에서 해당 컬럼 부분만 Slice
			ReadOnlySpan<char> idSpan = line[columnRanges[idIndex]];
			ReadOnlySpan<char> textSpan = line[columnRanges[langIndex]];

			// 이제 string으로 변환하면서 따옴표 처리 및 줄바꿈 문자 교체를 수행
			string id = UnquoteSpan(idSpan);
			// 줄바꿈 문자(\n)를 실제 개행으로 교체
			string text = UnquoteSpan(textSpan).Replace("\\n", "\n");

			if (!string.IsNullOrEmpty(id) && !dic.ContainsKey(id))
			{
				dic.Add(id, text);
			}
		}
		return dic;
	}

	/// <summary>
	/// Span에서 개행 문자를 기준으로 한 줄을 읽어냅니다.
	/// </summary>
	/// <param name="remaining">남은 텍스트 Span (읽은 후 갱신됨)</param>
	/// <param name="line">읽어낸 한 줄 Span</param>
	/// <returns>더 읽을 내용이 있으면 true, 없으면 false</returns>
	private bool TryReadLine(ref ReadOnlySpan<char> remaining, out ReadOnlySpan<char> line)
	{
		if (remaining.IsEmpty)
		{
			line = default;
			return false;
		}

		// 개행 문자(\n) 위치 검색
		int idx = remaining.IndexOf('\n');
		if (idx == -1)
		{
			// 파일 끝에 도달한 경우
			line = remaining.TrimEnd('\r');
			remaining = ReadOnlySpan<char>.Empty;
		}
		else
		{
			// 개행 문자 앞까지 자르고, 뒤의 \r 제거
			line = remaining.Slice(0, idx).TrimEnd('\r');
			// 남은 부분 갱신 (개행 문자 다음부터)
			remaining = remaining.Slice(idx + 1);
		}
		return true;
	}

	private bool IsWhiteSpaceSpan(ReadOnlySpan<char> span)
	{
		foreach (char c in span)
		{
			if (!char.IsWhiteSpace(c)) return false;
		}
		return true;
	}

	/// <summary>
	/// CSV 라인을 파싱하여 컬럼들을 string 리스트로 반환합니다. (헤더 처리용)
	/// 쉼표(,)를 기준으로 나누되, 따옴표(") 안의 내용은 하나의 컬럼으로 처리합니다.
	/// </summary>
	/// <param name="line">CSV 라인 Span</param>
	/// <param name="output">결과를 담을 리스트</param>
	private void SplitCsvLineToStrings(ReadOnlySpan<char> line, List<string> output)
	{
		int start = 0;
		bool insideQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			// 따옴표 상태 토글 (이중 따옴표 안의 쉼표는 구분자로 처리하지 않음)
			if (line[i] == '"') insideQuotes = !insideQuotes;
			else if (line[i] == ',' && !insideQuotes)
			{
				// 쉼표를 만나면 현재까지의 범위를 잘라 리스트에 추가
				output.Add(UnquoteSpan(line.Slice(start, i - start)));
				start = i + 1; // 다음 시작 위치 갱신
			}
		}
		// 마지막 컬럼 추가
		output.Add(UnquoteSpan(line.Slice(start)));
	}

	/// <summary>
	/// CSV 라인을 파싱하여 컬럼들의 Range(인덱스 범위)를 반환합니다.
	/// 실제 문자열 생성은 필요한 컬럼에 대해서만 나중에 수행합니다.
	/// </summary>
	/// <param name="line">CSV 라인 Span</param>
	/// <param name="ranges">Range 정보를 담을 리스트</param>
	private void SplitCsvLineToRanges(ReadOnlySpan<char> line, List<Range> ranges)
	{
		int start = 0;
		bool insideQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			if (line[i] == '"') insideQuotes = !insideQuotes;
			else if (line[i] == ',' && !insideQuotes)
			{
				ranges.Add(new Range(start, i));
				start = i + 1;
			}
		}
		ranges.Add(new Range(start, line.Length));
	}

	/// <summary>
	/// CSV 필드의 따옴표(")를 처리하고 문자열로 변환합니다.
	/// </summary>
	/// <param name="span">처리할 문자열 Span</param>
	/// <returns>따옴표가 제거된 문자열</returns>
	private string UnquoteSpan(ReadOnlySpan<char> span)
	{
		span = span.Trim();
		if (span.IsEmpty) return string.Empty;

		// 양 끝이 따옴표로 감싸져 있다면 제거
		if (span.Length >= 2 && span[0] == '"' && span[span.Length - 1] == '"')
		{
			span = span.Slice(1, span.Length - 2);
			// 내부의 이중 따옴표("")를 단일 따옴표(")로 변경
			// 이 경우 문자열 조작이 필요하므로 string 생성
			string s = span.ToString();
			return s.Replace("\"\"", "\"");
		}
		// 따옴표가 없다면 바로 string으로 변환
		return span.ToString();
	}

	/// <summary>
	/// CSV_Type과 ID로 텍스트를 가져옵니다. 없는 경우 ID 자체를 반환합니다.
	/// </summary>
	public string GetText(CSV_Type type, string id)
	{
		if (_tables.TryGetValue(type, out var table))
		{
			if (table.TryGetValue(id, out var value))
				return value;
		}
		return id;
	}

	public string GetFormatText(CSV_Type type, string id, params object[] args)
	{
		string template = GetText(type, id);

		try
		{
			return string.Format(template, args);
		}
		catch (FormatException e)
		{
			Debug.LogWarning($"[Localization] Format failed. ID: {id}, Text: {template}, Error: {e.Message}");
			return template;
		}
	}

	/// <summary>
	/// 언어를 변경하고 이벤트를 발동합니다.
	/// </summary>
	public void SetLanguage(Language lang)
	{
		if (currentLanguage == lang) return;
		currentLanguage = lang;

		// 변경된 언어 설정 저장
		PlayerPrefs.SetString(languageSaveKey, lang.ToString());

		InitializeLocalization().Forget();
	}

	/// <summary>
	/// Language enum을 사람이 읽을 수 있는 언어 이름으로 변환하여 반환합니다. (예: KR -> "한국어", EN -> "English")
	/// </summary>
	/// <param name="lang"></param>
	/// <returns></returns>
	public string GetLanguageName(Language lang)
	{
		return lang switch
		{
			Language.KR => "한국어",
			Language.EN => "English",
			_ => lang.ToString(),
		};
	}
}