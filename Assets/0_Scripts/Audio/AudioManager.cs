using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potan.CoreUtils;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfx3DSourcePrefab;
    [SerializeField] private AudioSource ambient3DSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioDataSO audioDataSO;

    private readonly List<AudioSource> _audioSourcesPool = new();
    private readonly List<AudioSource> _ambient3DSourcesPool = new();
    private readonly HashSet<AudioSource> _activeAmbient3DSources = new();

    protected override void OnAwake()
    {
        if (audioDataSO == null)
        {
            Debug.LogError("AudioDataSO가 할당되지 않았습니다.", this);
            return;
        }

        audioDataSO.Initialize();
    }

    public AudioData GetData(string id)
    {
        return audioDataSO != null &&
               audioDataSO.TryGetAudioData(id, out var data)
            ? data
            : default;
    }

    public void PlaySfx2D(string id)
    {
        if (!TryGetData(id, out var data))
            return;

        sfx2DSource.spatialBlend = 0f;
        sfx2DSource.pitch = data.pitch;
        sfx2DSource.PlayOneShot(data.clip, data.volume);
    }

    public void PlaySfx3D(string id, Vector3 position)
    {
        if (!TryGetData(id, out var data))
            return;

        AudioSource source = GetAudioSource(position);

        source.Stop();
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.Play();

        ReturnWhenFinishedAsync(
            source,
            destroyCancellationToken
        ).Forget();
    }

    public void PlayBgm(string id)
    {
        if (!TryGetData(id, out var data))
            return;

        PlayBgm(data.clip, data.volume, data.pitch);
    }

    public void PlayBgm(
        AudioClip clip,
        float volume = 1f,
        float pitch = 1f)
    {
        if (clip == null)
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.pitch = pitch;
        bgmSource.spatialBlend = 0f;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public AudioSource PlayAmbient3D(string id, Vector3 position, float minDistance = 3f, float maxDistance = 20f)
    {
        if (!TryGetData(id, out AudioData data)) return null;

        AudioSource source = GetAmbient3DSource(position);

        if (source == null) return null;

        source.Stop();
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.spatialBlend = 1f;
        source.loop = true;
        source.playOnAwake = false;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0f;
        source.Play();

        _activeAmbient3DSources.Add(source);

        return source;
    }

    public void StopAmbient3D(AudioSource source)
    {
        if (source == null) return;

        if (!_activeAmbient3DSources.Remove(source)) return;

        source.Stop();
        source.clip = null;
        source.loop = false;
        source.transform.SetParent(transform);
        source.transform.localPosition = Vector3.zero;
        source.gameObject.SetActive(false);

        _ambient3DSourcesPool.Add(source);
    }

    public void StopAllAmbient3D()
    {
        AudioSource[] activeSources = new AudioSource[_activeAmbient3DSources.Count];
        _activeAmbient3DSources.CopyTo(activeSources);

        foreach (AudioSource source in activeSources)
        {
            StopAmbient3D(source);
        }
    }

    private AudioSource GetAudioSource(Vector3 position)
    {
        AudioSource source;

        if (_audioSourcesPool.Count > 0)
        {
            int lastIndex = _audioSourcesPool.Count - 1;
            source = _audioSourcesPool[lastIndex];
            _audioSourcesPool.RemoveAt(lastIndex);
        }
        else
        {
            source = Instantiate(
                sfx3DSourcePrefab,
                position,
                Quaternion.identity,
                transform);
        }

        source.transform.position = position;
        source.gameObject.SetActive(true);

        return source;
    }

    private AudioSource GetAmbient3DSource(Vector3 position)
    {
        AudioSource source;

        if (_ambient3DSourcesPool.Count > 0)
        {
            int lastIndex = _ambient3DSourcesPool.Count - 1;
            source = _ambient3DSourcesPool[lastIndex];
            _ambient3DSourcesPool.RemoveAt(lastIndex);
        }
        else
        {
            if (ambient3DSourcePrefab == null)
            {
                Debug.LogError("[AudioManager] Ambient 3D AudioSource 프리팹이 없습니다.", this);
                return null;
            }

            source = Instantiate(ambient3DSourcePrefab, position, Quaternion.identity, transform);
        }

        source.transform.SetParent(transform);
        source.transform.position = position;
        source.gameObject.SetActive(true);

        return source;
    }

    private async UniTaskVoid ReturnWhenFinishedAsync(
        AudioSource source,
        CancellationToken cancellationToken)
    {
        bool canceled = await UniTask
            .WaitUntil(source,
                (state) => state == null || !state.isPlaying,
                cancellationToken: cancellationToken)
            .SuppressCancellationThrow();

        if (canceled || source == null)
            return;

        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);

        _audioSourcesPool.Add(source);
    }

    private bool TryGetData(string id, out AudioData data)
    {
        data = default;

        if (audioDataSO == null)
        {
            Debug.LogError("[AudioManager] AudioDataSO가 할당되지 않았습니다.");
            return false;
        }

        if (!audioDataSO.TryGetAudioData(id, out data))
        {
            Debug.LogWarning($"[AudioManager] ID를 찾을 수 없습니다: {id}");
            return false;
        }

        if (data.clip == null)
        {
            Debug.LogWarning(
                $"[AudioManager] {id}에 매핑된 AudioClip이 없습니다.");
            return false;
        }

        return true;
    }
}