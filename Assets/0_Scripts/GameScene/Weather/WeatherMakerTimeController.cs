using UnityEngine;
using DigitalRuby.WeatherMaker;
using System.Collections;
using Unity.VisualScripting;

public class WeatherMakerTimeController : MonoBehaviour
{
    public static WeatherMakerTimeController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public int GetCurHTime()
    {
        WeatherMakerDayNightCycleManagerScript cycle =
           WeatherMakerDayNightCycleManagerScript.Instance;

        if (cycle == null)
        {
            Debug.LogError("DayNightCycleManager을 찾을 수 없습니다.");
            return 0;
        }

        float timeOfDay = cycle.TimeOfDay;

        int hour = Mathf.FloorToInt(timeOfDay / 3600f);
        return hour;
    }

    public void SetTime(int hour, int minute = 0)
    {
        WeatherMakerDayNightCycleManagerScript cycle =
            WeatherMakerDayNightCycleManagerScript.Instance;

        if (cycle == null)
        {
            Debug.LogError("DayNightCycleManager을 찾을 수 없습니다.");
            return;
        }

        hour = Mathf.Clamp(hour, 0, 23);
        minute = Mathf.Clamp(minute, 0, 59);

        cycle.TimeOfDay = (hour * 3600f) + (minute * 60f);
    }

    public void SetTimeSpeed(float speed)
    {
        WeatherMakerDayNightCycleManagerScript cycle =
            WeatherMakerDayNightCycleManagerScript.Instance;

        if (cycle == null)
        {
            Debug.LogError("DayNightCycleManager을 찾을 수 없습니다.");
            return;
        }

        cycle.Speed = speed;
    }

    public void PlayTimeTransition(int sHour, int eHour, float duration)
    {
        StartCoroutine(IPlayTimeTransition(sHour, eHour, duration));
    }

    private IEnumerator IPlayTimeTransition(int sHour, int eHour, float duration)
    {
        sHour = Mathf.Clamp(sHour, 0, 23);
        eHour = Mathf.Clamp(eHour, 0, 23);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float ratio = Mathf.Clamp01(elapsed / duration);
            ratio = Mathf.SmoothStep(0f, 1f, ratio);

            int curHour = Mathf.RoundToInt(Mathf.Lerp(sHour, eHour, ratio));

            SetTime(curHour, 0);

            yield return null;
        }

        SetTime(eHour, 0);
    }

    public void SetMorning()
    {
        SetTime(6, 0);
    }

    public void SetNoon()
    {
        SetTime(12, 0);
    }

    public void SetEvening()
    {
        SetTime(18, 0);
    }

    public void SetNight()
    {
        SetTime(22, 0);
    }
}
