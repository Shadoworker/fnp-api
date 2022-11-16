using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PauseMenuController : MonoBehaviour
{
    public GameObject m_trollmanObj, m_shibaObj, m_saltBae, m_khabyLame, m_cube, m_grumpyCat;

    public void ActivateCube()
    {
        DesactivateAll();
        ActivateObj(m_cube);
    }

    public void SpawnCharacter(string _character)
    {
        GameStateManager.CharactersManager.DestroyCurrentPlayer();
        GameStateManager.CharactersManager.SpawnCharacter(_character);
    }

    public void ActivateTrollman()
    {
        DesactivateAll();
        ActivateObj(m_trollmanObj);
    }

    public void ActivateShiba()
    {
        DesactivateAll();
        ActivateObj(m_shibaObj);
    }
    public void ActivateKhaby()
    {
        DesactivateAll();
        ActivateObj(m_khabyLame);
    }
    public void ActivateSaltBae()
    {
        DesactivateAll();
        ActivateObj(m_saltBae);
    }
    public void ActivateGrumpyCat()
    {
        DesactivateAll();
        ActivateObj(m_grumpyCat);
    }

    public void DesactivateAll()
    {
        m_trollmanObj.SetActive(false);
        m_shibaObj.SetActive(false);
        m_saltBae.SetActive(false);
        m_khabyLame.SetActive(false);
        m_cube.SetActive(false);
        m_grumpyCat.SetActive(false);
    }

    public void ActivateObj(GameObject _activated)
    {
        DesactivateAll();
        _activated.SetActive(true);
        GameStateManager.CharactersManager.SetCurrentCharacter(_activated);
        GameStateManager.CameraManager.SetTarget(_activated);
    }
}
