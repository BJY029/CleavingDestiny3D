using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potan.CoreUtils;
using UnityEngine;
using System;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfx3DSourcePrefab;
    [SerializeField] private AudioSource ambient3DSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioDataSO audioDataSO;

    private struct BgmPlaybackState
    {
        public string id;
        public AudioClip clip;
        public float volume;
        public float pitch;
        public int timeSamples;
        public bool loop;
    }

    private readonly Stack<BgmPlaybackState> _bgmHistory = new();
    private CancellationTokenSource _bgmTransitionCancellationTokenSource;

    private string _currentBgmId;
    private float _currentBgmBaseVolume = 1f;

    private readonly List<AudioSource> _audioSourcesPool = new();
    private readonly List<AudioSource> _ambient3DSourcesPool = new();
    private readonly HashSet<AudioSource> _activeAmbient3DSources = new();

    private CancellationTokenSource _bgmFadeCancellationTokenSource;

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

        _bgmHistory.Clear();

        StartBgmTransition(id, data.clip, data.volume, data.pitch, 0, true, 0f, 0f);
    }

    public void PlayBgm(
        AudioClip clip,
        float volume = 1f,
        float pitch = 1f)
    {
        if (clip == null)
            return;

        _bgmHistory.Clear();

        CancelBgmFade();

        StartBgmTransition(null, clip, volume, pitch, 0, true, 0f, 0f);
    }

    public void PlayBgmFadeIn(string id, float fadeDuration = 1f)
    {
        if (!TryGetData(id, out AudioData data)) return;

        _bgmHistory.Clear();

        StartBgmTransition(id, data.clip, data.volume, data.pitch, 0, true, 0f, fadeDuration);
    }

    public void PlayTemporaryBgm(string id, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f, bool resumePreviousPosition = true)
    {
        if (!TryGetData(id, out AudioData data)) return;

        if (string.Equals(_currentBgmId, id, StringComparison.Ordinal)) return;

        if (bgmSource.clip != null)
        {
            BgmPlaybackState currentState = CaptureCurrentBgmState(resumePreviousPosition);
            _bgmHistory.Push(currentState);
        }

        StartBgmTransition(id, data.clip, data.volume, data.pitch, 0, true, fadeOutDuration, fadeInDuration);
    }

    public void RestorePreviousBgm(float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f)
    {
        if (_bgmHistory.Count == 0)
        {
            Debug.LogWarning("[AudioManager] 복원할 이전 BGM이 없습니다.", this);
            return;
        }

        BgmPlaybackState previousState = _bgmHistory.Pop();

        if (previousState.clip == null)
            return;

        StartBgmTransition(
            previousState.id,
            previousState.clip,
            previousState.volume,
            previousState.pitch,
            previousState.timeSamples,
            previousState.loop,
            fadeOutDuration,
            fadeInDuration
        );
    }

    private BgmPlaybackState CaptureCurrentBgmState(bool savePlaybackPosition)
    {
        int savedTimeSamples = 0;

        if (savePlaybackPosition && bgmSource.clip != null)
            savedTimeSamples = bgmSource.timeSamples;

        return new BgmPlaybackState
        {
            id = _currentBgmId,
            clip = bgmSource.clip,
            volume = _currentBgmBaseVolume,
            pitch = bgmSource.pitch,
            timeSamples = savedTimeSamples,
            loop = bgmSource.loop,
        };
    }

    private void StartBgmTransition(string id, AudioClip clip, float targetVolume, float pitch, int startTimeSamples, bool loop, float fadeOutDuration, float fadeInDuration)
    {
        if (clip == null)
            return;

        CancelBgmTransition();

        _bgmTransitionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        TransitionBgmAsync(
            id,
            clip,
            targetVolume,
            pitch,
            startTimeSamples,
            loop,
            fadeOutDuration,
            fadeInDuration,
            _bgmTransitionCancellationTokenSource.Token
        ).Forget();
    }

    private async UniTaskVoid TransitionBgmAsync(string id, AudioClip clip, float targetVolume, float pitch, int startTimeSamples, bool loop, float fadeOutDuration, float fadeInDuration, CancellationToken cancellationToken)
    {
        if (bgmSource == null)
            return;

        if (bgmSource.isPlaying && bgmSource.clip != null)
        {
            bool fadeOutCompleted = await FadeBgmVolumeAsync(0f, fadeOutDuration, cancellationToken);

            if (!fadeOutCompleted)
                return;
        }

        if (cancellationToken.IsCancellationRequested)
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.pitch = pitch;
        bgmSource.spatialBlend = 0f;
        bgmSource.loop = loop;

        if (startTimeSamples > 0 && clip.samples > 0)
        {
            int maximumSample = Mathf.Max(0, clip.samples - 1);
            bgmSource.timeSamples = Mathf.Clamp(startTimeSamples, 0, maximumSample);
        }

        _currentBgmId = id;
        _currentBgmBaseVolume = targetVolume;

        bgmSource.Play();

        await FadeBgmVolumeAsync(targetVolume, fadeInDuration, cancellationToken);
    }

    private async UniTask<bool> FadeBgmVolumeAsync(float targetVolume, float duration, CancellationToken cancellationToken)
    {
        if (bgmSource == null)
            return false;

        if (duration <= 0f)
        {
            bgmSource.volume = targetVolume;
            return true;
        }

        float startVolume = bgmSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / duration);
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);

            bool canceled = await UniTask
                .Yield(PlayerLoopTiming.Update, cancellationToken)
                .SuppressCancellationThrow();

            if (canceled)
                return false;
        }

        bgmSource.volume = targetVolume;
        return true;
    }

    public void StopBgmFadeOut(float fadeOutDuration = 1f)
    {
        CancelBgmTransition();

        _bgmTransitionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        StopBgmFadeOutAsync(fadeOutDuration, _bgmTransitionCancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid StopBgmFadeOutAsync(float fadeOutDuration, CancellationToken cancellationToken)
    {
        bool fadeCompleted = await FadeBgmVolumeAsync(0f, fadeOutDuration, cancellationToken);

        if (!fadeCompleted)
            return;

        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = 0f;

        _currentBgmId = null;
        _currentBgmBaseVolume = 1f;
        _bgmHistory.Clear();
    }

    private void CancelBgmFade()
    {
        if (_bgmFadeCancellationTokenSource == null)
            return;

        _bgmFadeCancellationTokenSource.Cancel();
        _bgmFadeCancellationTokenSource.Dispose();
        _bgmFadeCancellationTokenSource = null;
    }

    public void StopBgm()
    {
        CancelBgmTransition();

        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = 0f;

        _currentBgmId = null;
        _currentBgmBaseVolume = 1f;
        _bgmHistory.Clear();
    }

    private void CancelBgmTransition()
    {
        if (_bgmTransitionCancellationTokenSource == null)
            return;

        _bgmTransitionCancellationTokenSource.Cancel();
        _bgmTransitionCancellationTokenSource.Dispose();
        _bgmTransitionCancellationTokenSource = null;
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