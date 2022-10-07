using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

public enum GAME_MODE
{
    NORMAL,
    V_LEAGUE
}

public enum GAME_STATE
{
    DEFAULT_STATE,
    WIN_STATE,
    FAIL_STATE
}

public enum TIME_SCALE
{
    SLOW, NORMAL, FAST
}

[CreateAssetMenu(fileName = "Application Manager", menuName = "Managers/Application Manager")]
public class ApplicationManager : ScriptableObject
{
    private bool m_isLevelLoaded;
    [BoxGroup("Game State Events")]
    public StringEvent m_onGameEventModified;
    [BoxGroup("Game State Events")]
    public StringEvent m_onVLeagueGameEventModified;
    [BoxGroup("Game State Events")]
    public GameEvent m_onTempBoostPanelOpened;
    [BoxGroup("Game State Events")]
    public GameEvent m_onUpgradeSystemPanelOpened;
    public GameEvent m_onRealLifeSimulationStarted;
    private int m_playTime, m_seconds, m_minutes, m_hours, m_days = 0;
    private bool m_isDragAndDropFeatureEnabled = true;
    private bool m_existSaveGame = false;
    private List<float> m_timeScales = new List<float>() { 0.5f, 1f, 4f };
    public GAME_MODE m_currentGameMode;

    public void SetCurrentGameMode(string _gameMode)
    {
        GAME_MODE mode = Enum.GetValues(typeof(GAME_MODE)).Cast<GAME_MODE>().ToList().Where(e => e.ToString().ToLower().Equals(_gameMode.ToLower())).FirstOrDefault();
        m_currentGameMode = mode;
    }

    private void SetCurrentTimeScale()
    {
        Time.timeScale = m_timeScales[(int)m_currentTimeScale];
    }

    public void SetTimeScale(string _timeScale)
    {
        TIME_SCALE timeScale =  Enum.GetValues(typeof(TIME_SCALE)).Cast<TIME_SCALE>().ToList().Where(e => e.ToString().ToLower().Equals(_timeScale.ToLower())).FirstOrDefault();
        m_currentTimeScale = timeScale;
        Time.timeScale = m_timeScales[(int)m_currentTimeScale];
    }

    public TIME_SCALE GetCurrentGameSpeed()
    {
        return m_currentTimeScale;
    }

    public bool SaveGame
    {
        get { return PlayerPrefs.GetInt("m_existSaveGame") == 1 ? true : false; }
        set
        {
            m_existSaveGame = value;
            PlayerPrefs.SetInt("m_existSaveGame", m_existSaveGame == true ? 1: 0);
        }
    }

    [Space, BoxGroup("Cheats"), InlineButton("SetCurrentLVLCheat", "Set", false, 60), SerializeField, Label("Set Current Lvl")] private int currentLvl;

    [Space, BoxGroup("Time Scale"), InlineButton("SetCurrentTimeScale", "Set", false, 60), SerializeField, Label("Set Current Time Scale")] private TIME_SCALE m_currentTimeScale;

    public IEnumerator PlayTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            m_playTime += 1;
            m_seconds = (m_playTime % 60);
            m_minutes = (m_playTime / 60) % 60;
            m_hours = (m_playTime / 3600) % 24;
            m_days = (m_playTime / 86400) % 365;
        }
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
        PlayerPrefs.DeleteAll();
        UnPauseTime();
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    public void UnPauseTime()
    {
        Time.timeScale = m_timeScales[(int)m_currentTimeScale];
    }
    public void BuildStuff(RectTransform s_rec)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(s_rec);
    }

    public void InitGameState()
    {
        m_onGameEventModified.Raise(GAME_STATE.DEFAULT_STATE.ToString());
    }

    public void SetWinState()
    {
        if(m_currentGameMode == GAME_MODE.NORMAL)
            m_onGameEventModified.Raise(GAME_STATE.WIN_STATE.ToString());
        else if(m_currentGameMode == GAME_MODE.V_LEAGUE)
            m_onVLeagueGameEventModified.Raise(GAME_STATE.WIN_STATE.ToString());
    }

    public void SetFailState()
    {
        if (m_currentGameMode == GAME_MODE.NORMAL)
            m_onGameEventModified.Raise(GAME_STATE.FAIL_STATE.ToString());
        else if (m_currentGameMode == GAME_MODE.V_LEAGUE)
            m_onVLeagueGameEventModified.Raise(GAME_STATE.FAIL_STATE.ToString());
    }

    public GAME_STATE GetCurrentGameState()
    {
        if (m_onGameEventModified.m_previousValue.Equals(GAME_STATE.WIN_STATE.ToString()))
            return GAME_STATE.WIN_STATE;
        else if (m_onGameEventModified.m_previousValue.Equals(GAME_STATE.FAIL_STATE.ToString()))
            return GAME_STATE.FAIL_STATE;
        else
            return GAME_STATE.DEFAULT_STATE;
    }

    public void ToggleDragAndDropFeature(bool _toggle)
    {
        m_isDragAndDropFeatureEnabled = _toggle;
    }

    void OnApplicationQuit()
    {
        m_onGameEventModified.Raise(GAME_STATE.DEFAULT_STATE.ToString());
    }

    public bool IsDragAndDropFeatureEnabled()
    {
        return m_isDragAndDropFeatureEnabled;
    }

    [Button("Win Game")]
    private void WinGameCheat()
    {
        PauseTime();
        SetWinState();
    }


    [Button("Fail Game")]
    private void FailGameCheat()
    {
        PauseTime();
        SetFailState();
    }
    public bool HasSaveGame()
    {
        return SaveGame;
    }

    public void SetSaveGame()
    {
        SaveGame = true;
    }
    public void ResetSaveGame()
    {
        SaveGame = false;
    }
}
