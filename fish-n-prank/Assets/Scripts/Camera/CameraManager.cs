using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Managers/Camera Manager")]
public class CameraManager : ScriptableObject
{

    [FancyHeader("  Camera Manager SO  ", 1.5f, "yellow", 5.5f, order = 0)]
    [Label("")] public Empty e;
    private const float BOAT_CENTER_CAM_REF = 0.93f;
    private const float BOAT_CAM_DELTA_REF = 0.001f;
    [BoxGroup("Joystick settings")] public float m_sensivityX = 5.0f;
    [BoxGroup("Joystick settings")] public float m_boatSensivityX = 0.3f;
    [BoxGroup("Joystick settings")] public float m_sensivityY = 5.0f;
    [BoxGroup("Player Follow settings")] public float m_cameraRotationSensitivity = 0.6f;
    [HideInInspector] public CameraFollow m_cameraFollow;
    [HideInInspector] public float m_cameraXSensitivity;
    [BoxGroup("Camera transform")] public Vector3 m_characterCamPos = new Vector3(3, 5, 4);
    [BoxGroup("Camera transform")] public Vector3 m_initBoatCamPos = new Vector3(3, 10, 11);
    [BoxGroup("Camera transform")] public float m_boatRotationSensitivity = 0.9f;
    public bool m_isRotatingCamera;
    public bool m_isCameraCentered;
    public bool m_isLeftDamping = false;
    public bool m_isRightDamping = false;

    public void Init()
    {
        m_cameraFollow = Camera.main.GetComponent<CameraFollow>();
        m_isCameraCentered = false;
        m_isLeftDamping = false;
        m_isRightDamping = false;
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
    public void CenterCameraOnTarget(Transform _target)
    {
        var direction = (Camera.main.transform.position - _target.position).normalized;
        if (Mathf.Abs(Vector3.Dot(-_target.forward, direction) - BOAT_CENTER_CAM_REF) > BOAT_CAM_DELTA_REF && !IsCameraRotating())
        {
            if ((Mathf.Sign(Vector3.Dot(-_target.right, direction)) == 1 && !m_isRightDamping) || m_isLeftDamping)
            {
                m_isLeftDamping = true;
                m_cameraFollow.SetCurrentXValue(-m_boatRotationSensitivity * GameStateManager.CameraManager.m_boatSensivityX);
            }
            else if (!m_isLeftDamping)
            {
                m_isRightDamping = true;
                m_cameraFollow.SetCurrentXValue(m_boatRotationSensitivity * GameStateManager.CameraManager.m_boatSensivityX);
            }
        }
        else
        {
            m_isCameraCentered = true;
            m_isRightDamping = false;
            m_isLeftDamping = false;
        }
    }

    public void ToggleCameraRotation(bool _value)
    {
        m_isRotatingCamera = _value;
        if (_value)
            m_isCameraCentered = false;
    }

    public bool IsCameraRotating()
    {
        return m_isRotatingCamera;
    }
}
