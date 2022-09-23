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
    public float m_drag = 0.1f;

    //used Components
    protected Rigidbody m_rigidbody;
    protected Quaternion StartRotation;
    protected ParticleSystem ParticleSystem;

    //internal Properties
    protected Vector3 CamVel;
    [SerializeField] private FixedJoystick m_joystick = null;
    public Transform m_facingDirection;


    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        m_rigidbody = GetComponent<Rigidbody>();
        StartRotation = m_motor.localRotation;
    }

    public void FixedUpdate()
    {
        //default direction
        var forceDirection = transform.forward;
        var steer = 0;

        //steer direction [-1,0,1]
        if (Input.GetAxis("Horizontal") < 0 || m_joystick.m_horizontal < 0)
            steer = 1;
        if (Input.GetAxis("Horizontal") > 0 || m_joystick.m_horizontal > 0)
            steer = -1;


        //Rotational Force
        m_rigidbody.AddForceAtPosition(steer * transform.right * m_steerm_power / 100f, m_motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        var targetVel = Vector3.zero;

        //forward/backward poewr
        if (Input.GetAxis("Vertical") > 0 || m_joystick.m_vertical > 0)
            PhysicsHelper.ApplyForceToReachVelocity(m_rigidbody, forward * m_maxSpeed, m_power);
        if (Input.GetAxis("Vertical") < 0 || m_joystick.m_vertical < 0)
            PhysicsHelper.ApplyForceToReachVelocity(m_rigidbody, forward * -m_maxSpeed, m_power);

        //m_motor Animation // Particle system
        m_motor.SetPositionAndRotation(m_motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
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

        //camera position
        //Camera.transform.LookAt(transform.position + transform.forward * 6f + transform.up * 2f);
        //Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, transform.position + transform.forward * -8f + transform.up * 2f, ref CamVel, 0.05f);
    }

}
