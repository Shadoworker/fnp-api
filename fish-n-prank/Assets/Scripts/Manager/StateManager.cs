using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kayfo;

public class StateManager : Singleton<StateManager>
{ 
    
    [HideInInspector]
    public PersistentString m_persistentPlayerID = new PersistentString("persistentPlayerID", "");

    
    [HideInInspector]
    public PersistentString m_persistentPlayerResources = new PersistentString("persistentPlayerResources", "");


    void Start()
    {

    }
 
    public void GrantAccess()
    {
        SceneManager.LoadScene("GameplayMulti");
    }
}

 

