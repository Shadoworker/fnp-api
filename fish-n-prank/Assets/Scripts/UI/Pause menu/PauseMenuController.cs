using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PauseMenuController : MonoBehaviour
{
    public CameraFollow m_camera;
    public GameObject m_grumpyCatObj, m_shibaObj;

    public void ActivateGrumpyCat()
    {
        ActivateObj(m_grumpyCatObj, m_shibaObj);
    }

    public void ActivateShiba()
    {
        ActivateObj(m_shibaObj, m_grumpyCatObj);
    }

    public void ActivateObj(GameObject _activated, GameObject _disabled)
    {
        _disabled.SetActive(false);
        _activated.SetActive(true);
        m_camera.player = _activated;
    }
}
