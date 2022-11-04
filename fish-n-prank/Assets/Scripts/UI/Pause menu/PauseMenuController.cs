using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PauseMenuController : MonoBehaviour
{
    public CameraFollow m_camera;
    public GameObject m_trollmanObj, m_shibaObj, m_saltBae, m_khabyLame;
    public GameObject m_cameraBrain, m_virtualCamera;

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

    public void DesactivateAll()
    {
        m_trollmanObj.SetActive(false);
        m_shibaObj.SetActive(false);
        m_saltBae.SetActive(false);
        m_khabyLame.SetActive(false);
        m_virtualCamera.SetActive(false);
        m_cameraBrain.SetActive(false);
    }

    public void ActivateObj(GameObject _activated)
    {
        DesactivateAll();
        _activated.SetActive(true);
        GameStateManager.CharactersManager.SetCurrentCharacter(_activated);
        m_camera.player = _activated;
    }
}
