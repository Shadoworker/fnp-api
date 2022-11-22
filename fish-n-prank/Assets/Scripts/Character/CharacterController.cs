using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterController : NetworkBehaviour
{
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
    public CapsuleCollider m_capsuleCollider;
    RaycastHit m_objectHit;

    public void InitCharacterControllerValues()
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        InitCapsuleCollider(m_animator.gameObject.GetComponent<CapsuleCollider>());
        InittRigidbody(rigidbody);
        m_joystick = GameObject.Find("JoystickContainer").GetComponent<VariableJoystick>();
        m_rigidBody = rigidbody;
        m_characterData.m_characterSO.SetGroundedValue(true);
        InvokeRepeating("PlaySpecialIdle", 1.0f, m_characterData.m_characterSO.m_specialIdleRepeatRate);
    }

    public void InittRigidbody(Rigidbody _rigidbody)
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigidbody.angularDrag = m_characterData.m_characterSO.m_airAngularDrag;
        _rigidbody.drag = m_characterData.m_characterSO.m_dragForce;
        _rigidbody.maxAngularVelocity = m_characterData.m_characterSO.m_rotationSpeed;
        _rigidbody.drag = m_characterData.m_characterSO.m_dragForce;
        _rigidbody.mass = m_characterData.m_characterSO.m_mass;
    }

    public void InitCapsuleCollider(CapsuleCollider _copy)
    {
        m_capsuleCollider = GetComponent<CapsuleCollider>();
        m_capsuleCollider.height = _copy.height;
        m_capsuleCollider.center = _copy.center;
        m_capsuleCollider.radius = _copy.radius;
        _copy.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_characterData.m_characterSO.SetGroundedValue(true);
        m_triggerJump = false;
        m_characterData.m_characterSO.SetJumpInput(false);
    }

    public float GetYDistanceBetweenColliders(float _y1, float _y2)
    {
        return Mathf.Abs(_y1 - _y2);
    }

    public void PlaySpecialIdle()
    {
        if(m_animator != null)
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
            m_characterData.m_characterSO.SetGroundedValue(true);
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
            if (m_collisions.Count == 0 && !m_characterData.m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out m_objectHit, m_characterData.m_characterSO.m_rayCollisionRef)) { m_characterData.m_characterSO.SetGroundedValue(false);}
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0 && !m_characterData.m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out m_objectHit, m_characterData.m_characterSO.m_rayCollisionRef)) { m_characterData.m_characterSO.SetGroundedValue(false);}
    }

    private void FixedUpdate()
    {
        if (m_characterData.m_characterSO != null)
        {
            if (!m_characterData.m_characterSO.GetJumpInput() && (Input.GetKey(KeyCode.Space) || m_triggerJump))
            {
                m_characterData.m_characterSO.SetJumpInput(true);
            }
            DirectUpdate();
        }
        JumpingAndLanding();
        m_moveVector = Vector3.zero;
        if (m_characterData.m_characterSO != null && m_animator != null)
            m_animator.SetBool("Grounded", m_characterData.m_characterSO.IsGrounded());
        m_wasGrounded = m_characterData.m_characterSO.IsGrounded();
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
            if (!IsJumpCoolDownOver() || m_characterData.m_characterSO.IsGrounded())    // verified if the character is not jumping 
            {
                Move();
                transform.rotation = Quaternion.LookRotation(m_moveVector);
                if(m_animator)
                    m_animator.SetFloat("MoveSpeed", m_movement.magnitude);
            }
        }
        else if (!m_characterData.m_characterSO.GetJumpInput() && m_characterData.m_characterSO.IsGrounded())
        {
            m_rigidBody.velocity = Vector3.zero;
            if(m_animator != null)
                m_animator.SetFloat("MoveSpeed", 0);
        }
    }

    public void SetJumpInput()
    {
        m_triggerJump = true;
    }

    public void JumpingAndLanding()
    {
        if (IsJumpCoolDownOver() && m_characterData.m_characterSO.GetJumpInput() && m_characterData.m_buoyancyController != null && (m_characterData.m_characterSO.IsGrounded() || m_characterData.m_characterSO.IsUnderWater()))
        {
           m_jumpTimeStamp = Time.time;
            if(!m_characterData.m_buoyancyController.IsUnderwater())
                m_rigidBody.AddForce(((Vector3.up * m_characterData.m_characterSO.m_jumpForce) + m_moveVector), ForceMode.Impulse);
            else
                m_rigidBody.AddForce(((Vector3.up * m_characterData.m_characterSO.m_underwaterJumpForce) + m_moveVector), ForceMode.Impulse);
            m_characterData.m_characterSO.SetGroundedValue(false);
            m_characterData.m_characterSO.SetJumpInput(false);
        }

        if (m_animator && !m_wasGrounded && m_characterData.m_characterSO.IsGrounded())
        {
            m_animator.SetTrigger("Land");
        }

        if (m_animator && !m_characterData.m_characterSO.IsGrounded() && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    public bool IsJumpCoolDownOver()
    {
        return (Time.time - m_jumpTimeStamp) >= m_characterData.m_characterSO.m_minJumpInterval; ;
    }

    public void ToggleRunning(GameObject _runningIcon)
    {
        if(!m_characterData.m_characterSO.IsRunning())
        {
            m_characterData.m_characterSO.SetRunningValue(true);
            _runningIcon.GetComponent<Image>().color = Color.black;
        }
        else
        {
            m_characterData.m_characterSO.SetRunningValue(false);
            _runningIcon.GetComponent<Image>().color = Color.white;
        }
    }

    public void Move()
    {
        m_rigidBody.velocity = new Vector3(m_moveVector.x * m_moveSpeed, m_rigidBody.velocity.y, m_moveVector.z * m_moveSpeed);
    }

    public Vector3 PoolInput()
    {
        Vector3 dir = Vector3.zero;
        if(m_characterData.m_characterSO.m_movementMode == MovementMode.CONSTANT)
        {
            if (m_movement.magnitude < 1)
                m_moveSpeed = m_characterData.m_characterSO.m_walkSpeed;
            else
                m_moveSpeed = m_characterData.m_characterSO.m_runSpeed;
            dir.x = -m_joystick.m_horizontal;
            dir.z = -m_joystick.m_vertical;
        }
        else
        {
            m_moveSpeed = m_characterData.m_characterSO.m_runSpeed;
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
