using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Potan.CoreUtils;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;   // 마을씬 용 (2D)
    [SerializeField] private AudioSource bgmSource;
    
    [SerializeField] private AudioSource sfx3DSourcePrefab;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioDataSO audioDataSO;
    
    private readonly List<AudioSource> _audioSourcesPool = new List<AudioSource>();

    protected override void OnAwake()
    {
        if (audioDataSO != null)
        {
            audioDataSO.Initialize();
        }
        else
        {
            Debug.LogError("[AudioManager] AudioDataSO가 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// ID를 기반으로 AudioData 가져오기
    /// </summary>
    public AudioData GetData(string id)
    {
        if (audioDataSO != null && audioDataSO.TryGetAudioData(id, out var data))
        {
            return data;
        }
        return default;
    }

    /// <summary>
    /// ID를 기반으로 2D 효과음 재생
    /// </summary>
    public void PlaySfx2D(string id)
    {
        if (audioDataSO != null && audioDataSO.TryGetAudioData(id, out var data))
        {
            if (data.clip == null)
            {
                Debug.LogWarning($"[AudioManager] {id}에 매핑된 AudioClip이 없습니다.");
                return;
            }

            sfx2DSource.Stop();
            sfx2DSource.clip = data.clip;
            sfx2DSource.volume = data.volume;
            sfx2DSource.pitch = data.pitch;
            sfx2DSource.spatialBlend = 0f; // 2D 강제
            sfx2DSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] ID를 찾을 수 없습니다: {id}");
        }
    }

    /// <summary>
    /// ID를 기반으로 3D 공간 사운드 재생 (SO 자체 캐시 사용)
    /// </summary>
    public void PlaySfx3D(string id, Vector3 position)
    {
        if (audioDataSO != null && audioDataSO.TryGetAudioData(id, out var data))
        {
            if (data.clip == null)
            {
                Debug.LogWarning($"[AudioManager] {id}에 매핑된 AudioClip이 없습니다.");
                return;
            }

            PlaySfx3DInternal(data.clip, position, data.volume, data.pitch, data.is3D);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] ID를 찾을 수 없습니다: {id}");
        }
    }

    /// <summary>
    /// 기존 호환용 AudioClip 기반 3D 재생 메서드
    /// </summary>
    public void PlaySfx3D(AudioClip clip, Vector3 position)
    {
        PlaySfx3DInternal(clip, position, 1f, 1f, true);
    }

    private void PlaySfx3DInternal(AudioClip clip, Vector3 position, float volume, float pitch, bool is3D)
    {
        if (clip == null) return;

        AudioSource audioSource;
        if (_audioSourcesPool.Count > 0)
        {
            int lastIndex = _audioSourcesPool.Count - 1;
            audioSource = _audioSourcesPool[lastIndex];
            _audioSourcesPool.RemoveAt(lastIndex);
            
            audioSource.transform.position = position;
        }
        else
        {
            audioSource = Instantiate(sfx3DSourcePrefab, position, Quaternion.identity, transform);
        }
        
        audioSource.gameObject.SetActive(true);
        
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = is3D ? 1.0f : 0f; // 3D 여부 지정
        audioSource.Play();

        CheckAudioSource(audioSource, clip.length).Forget();
    }

    /// <summary>
    /// ID를 기반으로 BGM 재생 (SO 자체 캐시 사용)
    /// </summary>
    public void PlayBgm(string id)
    {
        if (audioDataSO != null && audioDataSO.TryGetAudioData(id, out var data))
        {
            PlayBgm(data.clip);
            bgmSource.volume = data.volume;
            bgmSource.pitch = data.pitch;
        }
        else
        {
            Debug.LogWarning($"[AudioManager] BGM ID를 찾을 수 없습니다: {id}");
        }
    }

    public void PlayBgm(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }

    private async UniTask CheckAudioSource(AudioSource source, float time)
    {
        await UniTask.WaitForSeconds(time);
        
        if (source != null)
        {
            _audioSourcesPool.Add(source);
            source.gameObject.SetActive(false);
        }
    }
}
