using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct AudioData
{
    [Tooltip("오디오 재생에 사용할 키(ID) 값입니다.")]
    public string id;
    
    [Tooltip("재생할 오디오 클립입니다.")]
    public AudioClip clip;
    
    [Range(0f, 1f)]
    [Tooltip("기본 볼륨 설정입니다.")]
    public float volume;
    
    [Range(0.1f, 3f)]
    [Tooltip("기본 피치 설정입니다.")]
    public float pitch;
    
    [Tooltip("3D 공간 음향 적용 여부입니다.")]
    public bool is3D;
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
        if (_audioCache == null) Initialize();
        return _audioCache.TryGetValue(id, out data);
    }
}
