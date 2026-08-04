using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct AudioData
{
    [Tooltip("오디오 재생에 사용할 키(ID) 값입니다.")]
    public string id;

    [Tooltip("재생할 오디오 클립입니다.")]
    public AudioClip clip;

    [Tooltip("랜덤 재생 후보로 사용할 추가 오디오 클립입니다.")]
    public AudioClip[] randomClips;

    [Tooltip("기본 클립과 추가 클립 중 하나를 랜덤으로 선택합니다.")]
    public bool useRandomClip;

    [Range(0f, 1f)]
    [Tooltip("기본 볼륨 설정입니다.")]
    public float volume;

    [Range(0.1f, 3f)]
    [Tooltip("기본 피치 설정입니다.")]
    public float pitch;

    [Tooltip("피치를 범위 내에서 랜덤하게 설정합니다.")]
    public bool useRandomPitch;

    [Tooltip("X는 최소 피치, Y는 최대 피치입니다.")]
    public Vector2 pitchRange;

    [Tooltip("3D 공간 음향 적용 여부입니다.")]
    public bool is3D;

    public AudioData GetRandomizedData()
    {
        AudioData result = this;
        if (useRandomClip) result.clip = GetRandomClip();

        if (useRandomPitch) result.pitch = GetRandomValue(pitchRange, 0.1f, 3f);

        return result;
    }

    private AudioClip GetRandomClip()
    {
        int vaildClipCount = clip != null ? 1 : 0;

        if (randomClips != null)
        {
            foreach (AudioClip randomClip in randomClips)
            {
                if (randomClip != null)
                    vaildClipCount++;
            }
        }

        if (vaildClipCount == 0) return null;

        int targetIndex = Random.Range(0, vaildClipCount);
        int curIdx = 0;

        if (clip != null)
        {
            if (curIdx == targetIndex) return clip;
            curIdx++;
        }

        if (randomClips != null)
        {
            foreach (AudioClip randomClip in randomClips)
            {
                if (randomClip == null) continue;
                if (curIdx == targetIndex) return randomClip;
                curIdx++;
            }
        }
        return clip;
    }

    private static float GetRandomValue(Vector2 range, float minimumLimit, float maximumLimit)
    {
        float min = Mathf.Clamp(Mathf.Min(range.x, range.y), minimumLimit, maximumLimit);
        float max = Mathf.Clamp(Mathf.Max(range.x, range.y), minimumLimit, maximumLimit);

        return Random.Range(min, max);
    }
}

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "Audio/AudioDataSO", order = 0)]
public class AudioDataSO : ScriptableObject
{
    [Tooltip("관리할 오디오 데이터 리스트입니다.")]
    public List<AudioData> audioDatas = new List<AudioData>();

    private Dictionary<string, AudioData> _audioCache;

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// 런타임 조회를 위해 데이터 리스트를 Dictionary 캐시로 초기화합니다.
    /// </summary>
    public void Initialize()
    {
        _audioCache = new Dictionary<string, AudioData>();
        foreach (var data in audioDatas)
        {
            if (string.IsNullOrEmpty(data.id)) continue;
            if (_audioCache.ContainsKey(data.id))
            {
                continue;
            }
            _audioCache.Add(data.id, data);
        }
    }

    /// <summary>
    /// ID를 기반으로 오디오 데이터를 조회합니다.
    /// </summary>
    public bool TryGetAudioData(string id, out AudioData data)
    {
        data = default;

        if (string.IsNullOrWhiteSpace(id))
            return false;

        if (_audioCache == null)
            Initialize();

        if (!_audioCache.TryGetValue(id, out AudioData cachedData))
            return false;

        data = cachedData.GetRandomizedData();
        return true;
    }
}
