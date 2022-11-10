using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;


[CreateAssetMenu(fileName = "Application Manager", menuName = "Managers/Application Manager")]
public class ApplicationManager : ScriptableObject
{
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
        PlayerPrefs.DeleteAll();
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }
    public void BuildStuff(RectTransform s_rec)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(s_rec);
    }
}
