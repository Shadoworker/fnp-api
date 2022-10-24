using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -180.0f;
    private const float Y_ANGLE_MAX = 20.0f;
    public GameObject player;
    public VariableJoystick m_cameraJoystick;
    public CameraSettings m_cameraSettings;
    private float m_currentX = 0.0f;
    private float m_currentY = 0.0f;
    public Vector3 m_offset;
    Vector3 m_cameraDirection;
    float m_cameraDistance;
    public Vector2 m_camDistanceMinMax = new Vector2(0.5f, 0.5f);
    private Vector3 m_dir = new Vector3(3f, 5, 4);

    private void Start()
    {
        m_cameraDirection = transform.localPosition.normalized;
        m_cameraDistance = m_camDistanceMinMax.y;
    }

    private void Update()
    {
        m_currentX += -m_cameraJoystick.m_horizontal * m_cameraSettings.m_sensivityX;
        m_currentY += -m_cameraJoystick.m_vertical * m_cameraSettings.m_sensivityY;
        m_currentY = ClampAngle(m_currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(m_currentY, m_currentX, 0);
        transform.position = player.transform.position + (rotation * m_dir);
        transform.LookAt(player.transform.position + m_offset);
        //CheckCameraOcclusionAndCollision();
    }

    public void SetCurrentXValue(float _value)
    {
        m_currentX += _value * m_cameraSettings.m_sensivityX;
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
