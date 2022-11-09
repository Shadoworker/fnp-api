using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{

    public CharacterSO m_characterSO;
    private float m_moveSpeed = 1;
    private Animator m_animator = null;
    private Rigidbody m_rigidBody = null;
    private VariableJoystick m_joystick = null;
    private List<Collider> m_collisions = new List<Collider>();
    Vector3 m_movement;
    public Vector3 m_moveVector { set; get; }
    private CameraFollow m_camTransform;
    private bool m_wasGrounded;
    private float m_jumpTimeStamp;
    public float m_cameraRotationScale = 0.6f;
    public BuoyancyObject m_buoyancyObject;
    public bool m_triggerJump;

    public void InitCharacterControllerValues()
    {
        gameObject.AddComponent<Rigidbody>();
        m_buoyancyObject = GetComponent<BuoyancyObject>();
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.GetComponent<Rigidbody>().angularDrag = m_characterSO.m_airAngularDrag;
        gameObject.GetComponent<Rigidbody>().drag = m_characterSO.m_dragForce;
        m_joystick = GameObject.Find("JoystickContainer").GetComponent<VariableJoystick>();
        if (!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { m_rigidBody = gameObject.GetComponent<Rigidbody>(); }
        m_rigidBody.maxAngularVelocity = m_characterSO.m_rotationSpeed;
        m_rigidBody.drag = m_characterSO.m_dragForce;
        m_rigidBody.mass = m_characterSO.m_mass;
        m_camTransform = Camera.main.transform.GetComponent<CameraFollow>();
        m_characterSO.SetGroundedValue(true);
        if (m_characterSO.m_character == CHARACTER.SHIBA)
            GameStateManager.CharactersManager.SetCurrentCharacter(gameObject);
        InvokeRepeating("PlaySpecialIdle", 1.0f, m_characterSO.m_specialIdleRepeatRate);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;

        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.01f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_characterSO.SetGroundedValue(true);
                m_triggerJump = false;
                m_characterSO.SetJumpInput(false);
            }
        }
    }

    public void PlaySpecialIdle()
    {
        m_animator.SetTrigger("Idle");
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.1f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_characterSO.SetGroundedValue(true);
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
            if (m_collisions.Count == 0 && !m_characterSO.IsUnderWater()) { m_characterSO.SetGroundedValue(false); }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0 && m_characterSO.GetJumpInput() && !m_characterSO.IsUnderWater()) { m_characterSO.SetGroundedValue(false); }
    }

    private void FixedUpdate()
    {
        if (m_characterSO != null)
        {
            if (!m_characterSO.GetJumpInput() && (Input.GetKey(KeyCode.Space) || m_triggerJump))
            {
                m_characterSO.SetJumpInput(true);
            }
            if (!m_characterSO.GetJumpInput() && m_characterSO.IsGrounded())
                DirectUpdate();
        }
        JumpingAndLanding();
        if (m_characterSO != null)
            m_animator.SetBool("Grounded", m_characterSO.IsGrounded());
        m_wasGrounded = m_characterSO.IsGrounded();
    }


    private void DirectUpdate()
    {
        m_movement = new Vector3(-m_joystick.m_horizontal, 0, -m_joystick.m_vertical);
        if (m_joystick.m_vertical != 0 || m_joystick.m_horizontal != 0)
        {
            m_camTransform.SetCurrentXValue(m_joystick.m_horizontal * m_camTransform.m_cameraSettings.m_cameraRotationSensitivity);
            m_moveVector = PoolInput(); //get the original input
            m_moveVector = RotateWithView();//rotate the player using our move vector
            Move();
            transform.rotation = Quaternion.LookRotation(m_moveVector);
            m_moveVector = Vector3.zero;
            m_animator.SetFloat("MoveSpeed", m_movement.magnitude);
        }
        else
        {
            m_rigidBody.velocity = Vector3.zero;
            m_animator.SetFloat("MoveSpeed", 0);
        }
    }

    public void SetJumpInput()
    {
        m_triggerJump = true;
    }

    public void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_characterSO.m_minJumpInterval;
        if (jumpCooldownOver && m_characterSO.GetJumpInput() && m_buoyancyObject != null && (m_characterSO.IsGrounded() || m_characterSO.IsUnderWater()))
        {
           m_jumpTimeStamp = Time.time;
            if(!m_buoyancyObject.IsUnderwater())
                m_rigidBody.AddForce((Vector3.up + m_moveVector) * m_characterSO.m_jumpForce, ForceMode.Impulse);
            else
                m_rigidBody.AddForce((Vector3.up + m_moveVector) * m_characterSO.m_diveForce, ForceMode.Impulse);
            m_characterSO.SetGroundedValue(false);
            m_characterSO.SetJumpInput(false);
        }

        if (!m_wasGrounded && m_characterSO.IsGrounded())
        {
            //m_characterSO.SetJumpInput(false);
            m_animator.SetTrigger("Land");
        }

        if (!m_characterSO.IsGrounded() && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    public void ToggleRunning(GameObject _runningIcon)
    {
        if(!m_characterSO.IsRunning())
        {
            m_characterSO.SetRunningValue(true);
            _runningIcon.GetComponent<Image>().color = Color.black;
        }
        else
        {
            m_characterSO.SetRunningValue(false);
            _runningIcon.GetComponent<Image>().color = Color.white;
        }
    }

    public void Move()
    {
        Vector3 dir = new Vector3(m_moveVector.x * GameStateManager.CharactersManager.m_xAxisSensitivity, m_moveVector.y, m_moveVector.z * GameStateManager.CharactersManager.m_zAxisSensitivity);
        m_rigidBody.AddForce(dir * m_moveSpeed);
        m_rigidBody.velocity = Vector3.ClampMagnitude(m_rigidBody.velocity, m_characterSO.m_maxSpeed);
    }

    public Vector3 PoolInput()
    {
        Vector3 dir = Vector3.zero;
        if(m_characterSO.m_movementMode == MovementMode.CONSTANT)
        {
            if (m_movement.magnitude < 1)
                m_moveSpeed = m_characterSO.m_walkSpeed;
            else
                m_moveSpeed = m_characterSO.m_runSpeed;
            dir.x = -m_joystick.m_horizontal;
            dir.z = -m_joystick.m_vertical;
        }
        else
        {
            m_moveSpeed = m_characterSO.m_runSpeed;
            dir.x = -m_joystick.m_horizontal;
            dir.z = -m_joystick.m_vertical;
        }
        if (dir.magnitude > 1)
            dir.Normalize();
        return dir;
    }

    private Vector3 RotateWithView()
    {
        if (m_camTransform != null)
        {
            Vector3 dir = m_camTransform.transform.TransformDirection(m_moveVector);
            dir.Set(-dir.x, 0, -dir.z);
            return dir.normalized * m_moveVector.magnitude;
        }
        else
        {
            m_camTransform = Camera.main.transform.GetComponent<CameraFollow>();
            return m_moveVector;
        }
    }
}
