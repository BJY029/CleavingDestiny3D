using Potan.CoreUtils;
using UnityEngine;

public class VFXManager : MonoSceneSingleton<VFXManager>
{
    private ParticleSystem cachedItemPS;

    public ParticleSystem[] predefinedEffects; // 미리 만들어진 타격 효과들

    public enum VFXIndex
    {
        Hit_Tree
    }

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < predefinedEffects.Length; i++)
        {
            predefinedEffects[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        InitCachedVFX();
    }

    // 코드 기반 파티클 생성
    private void InitCachedVFX()
    {
        // 1. 파티클 오브젝트를 생성하고 이 매니저의 자식으로 설정 (하이어라키 정리)
        GameObject vfxObject = new GameObject("VFX_Explosion_Cached");
        vfxObject.transform.SetParent(this.transform);

        cachedItemPS = vfxObject.AddComponent<ParticleSystem>();
        cachedItemPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = cachedItemPS.main;
        main.duration = 1.0f;
        main.loop = false;
        main.gravityModifier = 0.8f;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.None; // 스스로 파괴되지 않고 대기

        var shape = cachedItemPS.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // 크기가 커졌다가 작아지는 타격감 세팅
        var sizeModule = cachedItemPS.sizeOverLifetime;
        sizeModule.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(new Keyframe(0.0f, 0.0f));
        sizeCurve.AddKey(new Keyframe(0.2f, 1.0f));
        sizeCurve.AddKey(new Keyframe(1.0f, 0.0f));
        sizeModule.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);

        // 끝날 때 투명해지는 세팅
        var colorModule = cachedItemPS.colorOverLifetime;
        colorModule.enabled = true;
        Gradient alphaGradient = new Gradient();
        alphaGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.7f), new GradientAlphaKey(0f, 1f) }
        );
        colorModule.color = alphaGradient;

        var renderer = cachedItemPS.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
    }

    public void PlayPredefinedEffect(VFXIndex effect, Vector3 position)
    {
        int index = (int)effect;
        if (index < 0 || index >= predefinedEffects.Length)
        {
            Debug.LogWarning("Invalid effect index");
            return;
        }

        // 생성된 효과를 위치에 배치하고 재생
        // 프리팹이 아니라 생성된 파티클 시스템이므로, 위치만 이동시키고 재생하면 됩니다.
        cachedItemPS.transform.position = position;
        cachedItemPS.Play();
    }

    // 외부에서 아이템을 사용할 때마다 호출하는 함수
    public void PlayItemExplosion(Vector3 position, ItemClass itemClass)
    {
        if (cachedItemPS == null) return;

        // 1. 파티클 위치를 현재 아이템 위치로 이동
        cachedItemPS.transform.position = position;

        // 2. 등급별 변수 셋팅
        Color mainColor = Color.white;
        short particleCount = 20;
        float maxSpeed = 15f;

        // 아이템 Enum에 따라 색상, 파티클 수, 최대 속도 등을 다르게 설정
        switch (itemClass)
        {
            case ItemClass.Common:
                mainColor = new Color(0.8f, 0.8f, 0.8f); // 흰색/회색
                particleCount = 20;
                maxSpeed = 15f;
                break;
            case ItemClass.Rare:
                mainColor = new Color(0.2f, 0.8f, 1.0f); // 파란색/청록색
                particleCount = 40;
                maxSpeed = 25f;
                break;
            case ItemClass.Hero:
                mainColor = new Color(0.8f, 0.2f, 1.0f); // 보라색/마젠타
                particleCount = 70;
                maxSpeed = 35f;
                break;
            case ItemClass.Legendary:
                mainColor = new Color(1.0f, 0.6f, 0.1f); // 황금색/주황색
                particleCount = 120;
                maxSpeed = 50f;
                break;
        }

        // 3. 변하는 값들만 덮어쓰기 (할당 없이 값만 변경)
        var main = cachedItemPS.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, maxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = mainColor;

        var emission = cachedItemPS.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, particleCount) });

        // 4. 실행
        cachedItemPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cachedItemPS.Play();
    }
}