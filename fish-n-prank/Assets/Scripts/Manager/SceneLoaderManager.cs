using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using TMPro;
using Kayfo;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{ 
    
    public float SCENE_LOADER_DELAY = 0.10f;

    void Start()
    {
        StartCoroutine(ILoad());
    }

	private void Load()
	{
       string playerID = StateManager.Instance.m_persistentPlayerID.Get();

       if(playerID == "")
       {
            SceneManager.LoadScene("LoginScreen");
       }
       else
       {
            SceneManager.LoadScene("GameplayMulti");
       }
	}

    IEnumerator ILoad()
    {
        yield return new WaitForSeconds(SCENE_LOADER_DELAY);
        Load();
    }

 
 
}

 

