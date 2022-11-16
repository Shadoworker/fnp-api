using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Managers/Camera Manager")]
public class CameraManager : ScriptableObject
{

    [FancyHeader("  Camera Manager SO  ", 1.5f, "yellow", 5.5f, order = 0)]
    [Label("")] public Empty e;

    [BoxGroup("Joystick settings")] public float m_sensivityX = 5.0f;
    [BoxGroup("Joystick settings")] public float m_boatSensivityX = 0.3f;
    [BoxGroup("Joystick settings")] public float m_sensivityY = 5.0f;
    [BoxGroup("Player Follow settings")] public float m_cameraRotationSensitivity = 0.6f;
    [HideInInspector] public CameraFollow m_cameraFollow;
    [HideInInspector] public float m_cameraXSensitivity;
    [BoxGroup("Camera transform")] public Vector3 m_characterCamPos = new Vector3(3, 5, 4);
    [BoxGroup("Camera transform")] public Vector3 m_initBoatCamPos = new Vector3(3, 10, 11);
    [BoxGroup("Camera transform")] public Vector3 m_boatCamPos = new Vector3(3, 10, 11);
    [BoxGroup("Camera transform")] public Vector3 m_boatCamRot = new Vector3(3, 10, 11);

    public void Init()
    {
        m_cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void SetTarget(GameObject _target, bool _isCharacter = true, Transform _boat = null)
    {
        m_cameraFollow.m_target = _target;
        if(_isCharacter)
        {
            m_cameraXSensitivity = m_sensivityX;
            m_cameraFollow.m_dir = m_characterCamPos;
        }
        else
        {
            m_cameraXSensitivity = m_boatSensivityX;
            m_cameraFollow.m_dir = m_initBoatCamPos;
        }
    }
}
