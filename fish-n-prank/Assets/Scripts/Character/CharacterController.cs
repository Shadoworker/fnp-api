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
    [SerializeField] private float m_maxSpeed = 2;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private float m_diveForce = 4;
    
    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;
    [SerializeField] private VariableJoystick m_joystick = null;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;
    private bool m_wasGrounded;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 1.3f;
    private bool m_jumpInput = false;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();
    Vector3 m_movement;
    public bool m_tempIsShiba;
    //New input system
    public float m_drag = 0.5f;
    public float m_terminalRotationSpeed = 25.0f;
    public Vector3 m_moveVector { set; get; }
    private Transform m_camTransform;
    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Rigidbody>(); }
    }

    private void Start()
    {
        m_rigidBody.maxAngularVelocity = m_terminalRotationSpeed;
        m_rigidBody.drag = m_drag;
        m_camTransform = Camera.main.transform;
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
        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);


        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }


    private void DirectUpdate()
    {
        m_movement = new Vector3(-m_joystick.m_horizontal, 0, -m_joystick.m_vertical);
        if (m_joystick.m_vertical != 0 || m_joystick.m_horizontal != 0)
        {
            m_camTransform.GetComponent<CameraFollow>().SetCurrentXValue(m_joystick.m_horizontal);
            m_moveVector = PoolInput(); //get the original input
            m_moveVector = RotateWithView();//rotate the player using our move vector
            Move();
            transform.rotation = Quaternion.LookRotation(m_moveVector);
            m_animator.SetFloat("MoveSpeed", m_movement.magnitude);
        }
        else
        {
            m_rigidBody.velocity = Vector3.zero;
            m_animator.SetFloat("MoveSpeed", 0);
        }
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

    public void Move()
    {
        m_rigidBody.AddForce(m_moveVector * m_moveSpeed);
        m_rigidBody.velocity = Vector3.ClampMagnitude(m_rigidBody.velocity, m_maxSpeed);
    }

    public Vector3 PoolInput()
    {
        Vector3 dir = Vector3.zero;
        dir.x = -m_joystick.m_horizontal;
        dir.z = -m_joystick.m_vertical;
        if (dir.magnitude > 1)
            dir.Normalize();
        return dir;
    }

    private Vector3 RotateWithView()
    {
        if (m_camTransform != null)
        {
            Vector3 dir = m_camTransform.TransformDirection(m_moveVector);
            dir.Set(-dir.x, 0, -dir.z);
            return dir.normalized * m_moveVector.magnitude;
        }
        else
        {
            m_camTransform = Camera.main.transform;
            return m_moveVector;
        }
    }
}
