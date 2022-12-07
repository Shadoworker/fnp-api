using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameState Manager")]
#if UNITY_EDITOR
[FilePath("Scriptable Objects/Managers/GameStateManager.asset", FilePathAttribute.Location.PreferencesFolder)]
#endif
public class GameStateManager : SingletonScriptableObject<GameStateManager>
{
    [FancyHeader("GAMESTATE MANAGER", 3f, "#D4AF37", 8.5f, order = 0)]
    [Space(order = 1)]
    [CustomProgressBar(hideWhenZero = true, label = "m_loadingTxt"), SerializeField] public float m_loadingBar;
    [HideInInspector] public string m_loadingTxt;
    [HideInInspector] public bool m_loadingDone = false;

    [SerializeField] private CharactersManager m_charactersManager;

    [SerializeField] private CameraManager m_cameraManager;

    [SerializeField] private FishesManager m_fishesManager;

    public static FishesManager FishesManager
    {
        get { return Instance.m_fishesManager; }
    }

    public static CharactersManager CharactersManager
    {
        get { return Instance.m_charactersManager; }
    }

    public static CameraManager CameraManager
    {
        get { return Instance.m_cameraManager; }
    }

    public void Init()
    {
        Application.targetFrameRate = 100;
        CameraManager.Init();
        CharactersManager.Init();
    }

    [Button("Delete All Save Data", sp: 30)]
    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }
}
