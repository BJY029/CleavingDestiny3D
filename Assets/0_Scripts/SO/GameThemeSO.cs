using UnityEngine;
using DigitalRuby.WeatherMaker;

[CreateAssetMenu(fileName = "GameThemeSO", menuName = "Scriptable Objects/GameThemeSO")]
public class GameThemeSO : ScriptableObject
{
    [Header("Theme identification value")]
    [SerializeField] private string themeId;
    [SerializeField] private string displayName;
    [SerializeField] private GameTheme themeType;


    [Header("Weather Maker")]
    [SerializeField] private WeatherMakerProfileScript weatherProfile;

    [Header("Game Data")]
    [SerializeField] private PlayerSetting playerData;
    [SerializeField] private RoomSetting roomData;

    public string ThemeId => themeId;
    public string DisplayName => displayName;
    public GameTheme ThemeType => themeType;

    public WeatherMakerProfileScript WeatherProfile => weatherProfile;

    public PlayerSetting PlayerData => playerData;
    public RoomSetting RoomData => roomData;

#if UNITY_EDITOR
    private void OnValidate()
    {
        themeId = themeId?.Trim();

        if (string.IsNullOrEmpty(themeId))
            Debug.LogWarning($"[{name}]Theme ID가 비어있습니다.");
    }
#endif
}
