using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    //visible Properties
    public Transform m_motor;
    public float m_steerm_power = 500f;
    public float m_power = 5f;
    public float m_maxSpeed = 10f;
    public float m_reverseSpeed = 10f;
    public float m_drag = 0.1f;
    public float m_steerPower = 30f;

    //used Components
    protected Rigidbody m_rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;

    //internal Properties
    protected Vector3 CamVel;
    [SerializeField] private VariableJoystick m_joystick = null;
    public Transform m_facingDirection;
    private Vector3 m_moveVector { set; get; }

    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        m_rigidbody = GetComponent<Rigidbody>();
        StartRotation = m_motor.localRotation;
    }

    public void FixedUpdate()
    {
        float steer = 0;
        if (Input.GetAxis("Horizontal") != 0 || m_joystick.m_horizontal != 0)
        {
            steer -= m_joystick.m_horizontal;
            GameStateManager.CameraManager.ToggleCameraRotation(true);
        }
        else
        {
            GameStateManager.CameraManager.ToggleCameraRotation(false);
        }

        if (!GameStateManager.CameraManager.m_isCameraCentered)
            GameStateManager.CameraManager.CenterCameraOnTarget(transform);
        
        GameStateManager.CameraManager.m_cameraFollow.SetCurrentXValue(m_joystick.m_horizontal * GameStateManager.CameraManager.m_boatSensivityX);
        GameStateManager.CameraManager.IsBoatFacingCam(transform, ref steer, new Vector3(m_joystick.m_horizontal, 0, m_joystick.m_vertical));
        //Rotational Force
        m_rigidbody.AddForceAtPosition(steer * transform.right * m_steerm_power / 100f, m_motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        //forward/backward power
        if (Input.GetAxis("Vertical") > 0 || m_joystick.m_vertical > 0)
            PhysicsHelper.ApplyForceToReachVelocity(m_rigidbody, forward * m_maxSpeed, m_power);
        else if (Input.GetAxis("Vertical") < 0 || m_joystick.m_vertical < 0)
            m_rigidbody.velocity = -forward * m_reverseSpeed;

        //m_motor Animation // Particle system
        m_motor.SetPositionAndRotation(m_motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, m_steerPower * steer, 0));
        if (ParticleSystem != null)
        {
            if (Input.GetAxis("Vertical") > 0 || m_joystick.m_vertical > 0 || Input.GetAxis("Vertical") < 0 || m_joystick.m_vertical < 0)
                ParticleSystem.Play();
            else
                ParticleSystem.Pause();
        }

        //moving forward
        var movingForward = Vector3.Cross(transform.forward, m_rigidbody.velocity).y < 0;

        //move in direction
        m_rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(m_rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * m_drag, Vector3.up) * m_rigidbody.velocity;
    }
}
