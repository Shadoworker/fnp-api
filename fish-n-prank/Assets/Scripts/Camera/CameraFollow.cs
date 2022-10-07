using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -180.0f;
    private const float Y_ANGLE_MAX = 20.0f;
    public GameObject player;
    public VariableJoystick m_cameraJoystick;
    private float m_currentX = 0.0f;
    private float m_currentY = 0.0f;
    private float m_sensivityX = 4.0f;
    private float m_sensivityY = 1.0f;
    public Vector3 m_offset;

    private void Update()
    {
        m_currentX += -m_cameraJoystick.m_horizontal * m_sensivityX;
        m_currentY += -m_cameraJoystick.m_vertical * m_sensivityY;
        m_currentY = ClampAngle(m_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    void LateUpdate()
    {
        Vector3 dir = new Vector3(3f, 5, 4);
        Quaternion rotation = Quaternion.Euler(m_currentY, m_currentX, 0);
        transform.position = player.transform.position + (rotation * dir);
        transform.LookAt(player.transform.position + m_offset);

    }

    public void SetCurrentXValue(float _value)
    {
        m_currentX += _value * m_sensivityX;
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

}
