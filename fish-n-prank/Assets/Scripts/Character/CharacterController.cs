using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterController : NetworkBehaviour
{
    public CharacterSO m_characterSO;
    public CharacterData m_characterData;
    private float m_moveSpeed = 1;
    public Animator m_animator = null;
    private Rigidbody m_rigidBody = null;
    private VariableJoystick m_joystick = null;
    private List<Collider> m_collisions = new List<Collider>();
    Vector3 m_movement;
    public Vector3 m_moveVector { set; get; }
    private bool m_wasGrounded;
    private float m_jumpTimeStamp;
    public float m_cameraRotationScale = 0.6f;
    public bool m_triggerJump;
    private const float SOLID_SURFACE_COLLISION_REF = 0.001f;
    RaycastHit m_objectHit;

    public void InitCharacterControllerValues()
    {
        gameObject.AddComponent<Rigidbody>();
        m_characterData = GetComponent<CharacterData>();
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.GetComponent<Rigidbody>().angularDrag = m_characterSO.m_airAngularDrag;
        gameObject.GetComponent<Rigidbody>().drag = m_characterSO.m_dragForce;
        m_joystick = GameObject.Find("JoystickContainer").GetComponent<VariableJoystick>();
        if (!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { m_rigidBody = gameObject.GetComponent<Rigidbody>(); }
        m_rigidBody.maxAngularVelocity = m_characterSO.m_rotationSpeed;
        m_rigidBody.drag = m_characterSO.m_dragForce;
        m_rigidBody.mass = m_characterSO.m_mass;
        m_characterSO.SetGroundedValue(true);
        if (m_characterSO.m_character == CHARACTER.SHIBA)
        {
            if (isOwned)
            {
                GameStateManager.CharactersManager.SetCurrentCharacter(gameObject);
            }
        }
        InvokeRepeating("PlaySpecialIdle", 1.0f, m_characterSO.m_specialIdleRepeatRate);
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_characterSO.SetGroundedValue(true);
        m_triggerJump = false;
        m_characterSO.SetJumpInput(false);
    }

    public float GetYDistanceBetweenColliders(float _y1, float _y2)
    {
        return Mathf.Abs(_y1 - _y2);
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
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > SOLID_SURFACE_COLLISION_REF)
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
            if (m_collisions.Count == 0 && !m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out m_objectHit, m_characterSO.m_rayCollisionRef)) { m_characterSO.SetGroundedValue(false);}
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0 && !m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out m_objectHit, m_characterSO.m_rayCollisionRef)) { m_characterSO.SetGroundedValue(false);}
    }

    private void FixedUpdate()
    {
        if (m_characterSO != null)
        {
            if (!m_characterSO.GetJumpInput() && (Input.GetKey(KeyCode.Space) || m_triggerJump))
            {
                m_characterSO.SetJumpInput(true);
            }
            DirectUpdate();
        }
        JumpingAndLanding();
        m_moveVector = Vector3.zero;
        if (m_animator && m_characterSO != null)
            m_animator.SetBool("Grounded", m_characterSO.IsGrounded());
        m_wasGrounded = m_characterSO.IsGrounded();
    }

    [ClientCallback]
    private void DirectUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (m_joystick == null)
        {
            Debug.LogWarning("CharacterController.DirectUpdate: Joystick not ready yet");
            return;
        }

        if (m_joystick.m_vertical != 0 || m_joystick.m_horizontal != 0)
        {
            m_movement = new Vector3(-m_joystick.m_horizontal, 0, -m_joystick.m_vertical);
            GameStateManager.CameraManager.m_cameraFollow.SetCurrentXValue(m_joystick.m_horizontal * GameStateManager.CameraManager.m_cameraRotationSensitivity);
            m_moveVector = PoolInput(); //get the original input
            m_moveVector = RotateWithView();//rotate the player using our move vector
            if (!IsJumpCoolDownOver() || m_characterSO.IsGrounded())    // verified if the character is not jumping 
            {
                Move();
                transform.rotation = Quaternion.LookRotation(m_moveVector);
                m_animator.SetFloat("MoveSpeed", m_movement.magnitude);
            }
        }
        else if (!m_characterSO.GetJumpInput() && m_characterSO.IsGrounded())
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
        if (IsJumpCoolDownOver() && m_characterSO.GetJumpInput() && m_characterData.m_buoyancyController != null && (m_characterSO.IsGrounded() || m_characterSO.IsUnderWater()))
        {
           m_jumpTimeStamp = Time.time;
            if(!m_characterData.m_buoyancyController.IsUnderwater())
                m_rigidBody.AddForce(((Vector3.up * m_characterSO.m_jumpForce) + m_moveVector), ForceMode.Impulse);
            else
                m_rigidBody.AddForce(((Vector3.up * m_characterSO.m_underwaterJumpForce) + m_moveVector), ForceMode.Impulse);
            m_characterSO.SetGroundedValue(false);
            m_characterSO.SetJumpInput(false);
        }

        if (m_animator && !m_wasGrounded && m_characterSO.IsGrounded())
        {
            m_animator.SetTrigger("Land");
        }

        if (m_animator && !m_characterSO.IsGrounded() && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    public bool IsJumpCoolDownOver()
    {
        return (Time.time - m_jumpTimeStamp) >= m_characterSO.m_minJumpInterval; ;
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
        //Vector3 dir = new Vector3(m_moveVector.x * m_moveSpeed, m_rigidBody.velocity.y, m_moveVector.z * m_moveSpeed);
        m_rigidBody.velocity = new Vector3(m_moveVector.x * m_moveSpeed, m_rigidBody.velocity.y, m_moveVector.z * m_moveSpeed);
        //m_rigidBody.velocity = Vector3.ClampMagnitude(m_rigidBody.velocity, m_characterSO.m_maxSpeed);
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
        if (GameStateManager.CameraManager.m_cameraFollow != null)
        {
            Vector3 dir = GameStateManager.CameraManager.m_cameraFollow.transform.TransformDirection(m_moveVector);
            dir.Set(-dir.x, 0, -dir.z);
            return dir.normalized * m_moveVector.magnitude;
        }
        else
        {
            GameStateManager.CameraManager.m_cameraFollow = Camera.main.transform.GetComponent<CameraFollow>();
            return m_moveVector;
        }
    }
}
