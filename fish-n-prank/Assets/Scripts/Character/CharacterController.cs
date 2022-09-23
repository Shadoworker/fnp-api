using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    private enum ControlMode
    {
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// </summary>
        Direct
    }


    [SerializeField] private bool m_running;
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private float m_diveForce = 4;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;
    [SerializeField] private FixedJoystick m_joystick = null;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;
    private float m_currentVRot = 0;
    private float m_currentHRot = 0;

    private readonly float m_interpolation = 10;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 1.3f;
    private bool m_jumpInput = false;

    private bool m_isGrounded;

    private List<Collider> m_collisions = new List<Collider>();

    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;

        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        if (!m_jumpInput && Input.GetKey(KeyCode.Space))
        {
            m_jumpInput = true;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }


    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical") != 0 && Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") :
            m_joystick.m_vertical != 0 && m_joystick.m_vertical > 0? m_joystick.m_vertical : 0;
        float h = Input.GetAxis("Horizontal") != 0 ? Input.GetAxis("Horizontal") :
            m_joystick.m_horizontal != 0 ? m_joystick.m_horizontal : 0;

        Transform camera = Camera.main.transform;
        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);
        m_currentVRot = Mathf.Lerp(m_currentV, v, Time.deltaTime);
        m_currentHRot = Mathf.Lerp(m_currentH, h, Time.deltaTime);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;
        Vector3 directionR = camera.forward * m_currentVRot + camera.right * m_currentHRot;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;
        Vector3 directionRot = directionR.normalized * directionLength;
        if (direction != Vector3.zero && (v != 0 || h != 0))
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(Vector3.Slerp(m_currentDirection, directionRot, Time.deltaTime * 1f));
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
        else
            m_animator.SetFloat("MoveSpeed", 0);
        JumpingAndLanding();
    }

    public void SetJumpInput()
    {
        m_jumpInput = true;
    }
    public void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            if(!GetComponent<BuoyancyObject>().IsUnderwater())
                m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            else
                m_rigidBody.AddForce(Vector3.up * m_diveForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (m_jumpInput)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    public void ToggleRunning(GameObject _runningIcon)
    {
        if(!m_running)
        {
            m_running = true;
            _runningIcon.GetComponent<Image>().color = Color.black;
        }
        else
        {
            m_running = false;
            _runningIcon.GetComponent<Image>().color = Color.white;
        }
    }
}
