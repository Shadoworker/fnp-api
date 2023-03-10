using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -50f;
    private const float Y_ANGLE_MAX = 20.0f;
    public GameObject m_target;
    public VariableJoystick m_cameraJoystick;
    private float m_currentX = 0.0f;
    private float m_currentY = 0.0f;
    public Vector3 m_offset;
    Vector3 m_cameraDirection;
    float m_cameraDistance;
    public Vector2 m_camDistanceMinMax = new Vector2(0.5f, 0.5f);
    public Vector3 m_dir = new Vector3(3f, 5, 4);

    private void Start()
    {
        m_cameraDirection = transform.localPosition.normalized;
        m_cameraDistance = m_camDistanceMinMax.y;
    }

    private void Update()
    {
        m_currentX += m_cameraJoystick.m_horizontal * GameStateManager.CameraManager.m_sensivityX;
        m_currentY += m_cameraJoystick.m_vertical * GameStateManager.CameraManager.m_sensivityY;
        m_currentY = ClampAngle(m_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        if (m_cameraJoystick.m_horizontal != 0 || m_cameraJoystick.m_vertical != 0)
            GameStateManager.CameraManager.ToggleCameraRotation(true);
        else
            GameStateManager.CameraManager.ToggleCameraRotation(false);
    }

    void LateUpdate()
    {
        if(m_target != null)
        {
            Quaternion rotation = Quaternion.Euler(m_currentY, m_currentX, 0);
            transform.position = m_target.transform.position + (rotation * m_dir);
            transform.LookAt(m_target.transform.position + m_offset);
        }
    }

    public void SetCurrentXValue(float _value)
    {
        m_currentX += _value * GameStateManager.CameraManager.m_sensivityX;
    }
    public void SetCurrentYValue(float _value)
    {
        m_currentY += _value * GameStateManager.CameraManager.m_sensivityY;
    }

    private float ClampAngle(float _angle, float _min, float _max)
    {
        do
        {
            if (_angle < -360)
                _angle += 360;
            if (_angle > 360)
                _angle -= 360;
        } while (_angle < -360 || _angle > 360);
        if (_angle == 0)
            return -Y_ANGLE_MIN;
        return Mathf.Clamp(_angle, _min, _max);
    }

    public void CheckCameraOcclusionAndCollision()
    {
        Vector3 desiredCamPosition = transform.TransformPoint(m_cameraDirection * m_camDistanceMinMax.y);
        RaycastHit hit;
        Debug.DrawLine(transform.position, desiredCamPosition, Color.red);
        if(Physics.Linecast(transform.position, desiredCamPosition, out hit))
            m_cameraDistance = Mathf.Clamp(hit.distance, m_camDistanceMinMax.x, m_camDistanceMinMax.y);
        else
            m_cameraDistance = m_camDistanceMinMax.y;
        transform.localPosition = m_cameraDirection * m_cameraDistance;
    }

}
