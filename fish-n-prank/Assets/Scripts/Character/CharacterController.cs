using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterController : NetworkBehaviour
{
    private const float MAX_RAY_DISTANCE = 40f;
    private const float MIN_JOYSTICK_OFFSET = 0.6f;
    private const float MAX_JOYSTICK_OFFSET = 1f;
    private const float SOLID_SURFACE_COLLISION_REF = 0.001f;
    private const string NAVIGATE_ANIM_PARAM = "Navigate"; // Centralize characters animations

    public CharacterData m_characterData;
    [HideInInspector]public GameObject m_playerHeadObj;
    private float m_moveSpeed = 1;
    public Animator m_animator = null;
    [HideInInspector] public Rigidbody m_rigidBody = null;
    private VariableJoystick m_joystick = null;
    private List<Collider> m_collisions = new List<Collider>();
    Vector3 m_movement;
    public Vector3 m_moveVector { set; get; }
    private bool m_wasGrounded;
    private float m_jumpTimeStamp;
    public float m_cameraRotationScale = 0.6f;
    public bool m_triggerJump;
    public CapsuleCollider m_capsuleCollider;
    Vector3 m_oldPosition;
    private float m_joystickOffset;
    Transform m_boatSeatTransform = null;
    bool m_isDolphinJump;

    public CharacterControlState State { get; private set; }  = CharacterControlState.Walking;

    public enum CharacterControlState
    {
        Walking, // TODO: add state for Swimming
        Sailing,
    }

    public void InitCharacterControllerValues()
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        InitCapsuleCollider(m_animator.gameObject.GetComponent<CapsuleCollider>());
        InitRigidbody(rigidbody);
        m_joystick = InputManager.Instance.MovementJoystick;
        m_rigidBody = rigidbody;
        m_characterData.m_characterSO.SetGroundedValue(true);
        InvokeRepeating("PlaySpecialIdle", 1.0f, m_characterData.m_characterSO.m_specialIdleRepeatRate);
        m_joystickOffset = MAX_JOYSTICK_OFFSET;
        StartCoroutine(GeneratePlayerHeadObj());
    }

    public void InitRigidbody(Rigidbody _rigidbody)
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
        m_capsuleCollider.material = _copy.material;
        _copy.enabled = false;
    }

    public void SetIsWalking()
    {
        GameStateManager.CameraManager.SetTarget(gameObject);
        m_characterData.m_animator.SetBool(NAVIGATE_ANIM_PARAM, false);
        //transform.SetParent(null);
        GetComponent<Rigidbody>().isKinematic = false;
        m_boatSeatTransform = null;

        State = CharacterControlState.Walking;
    }

    public void SetIsSailing(BoatController _boatController, Transform _boatSeatTransform)
    {
        transform.LookAt(_boatController.m_facingDirection);
        //transform.SetParent(_boatController.transform);
        transform.localPosition = _boatSeatTransform.localPosition;
        transform.rotation = _boatSeatTransform.rotation;
        m_characterData.m_animator.SetBool(NAVIGATE_ANIM_PARAM, true);
        GameStateManager.CameraManager.SetTarget(_boatController.gameObject, false, _boatController.transform);
        GetComponent<Rigidbody>().isKinematic = true;
        m_boatSeatTransform = _boatSeatTransform;

        State = CharacterControlState.Sailing;
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_triggerJump = false;
        m_characterData.m_characterSO.SetJumpInput(false);
        GetSpeed();
        if (collision.gameObject.tag != "Ground" && Mathf.Abs(m_movement.z) >= GameStateManager.CharactersManager.m_zJoystickOffset && !m_animator.GetBool("Land") && !m_characterData.m_characterSO.IsUnderWater() && !m_isDolphinJump)
        {
            Vector3 fwd = m_playerHeadObj.transform.TransformDirection(m_characterData.m_characterSO.m_jumpingRay);
            RaycastHit objectHit; // never used, make it local?
            Debug.DrawRay(m_playerHeadObj.transform.position, fwd, Color.yellow);
            if ((!Physics.Raycast(m_playerHeadObj.transform.position, fwd, out objectHit, MAX_RAY_DISTANCE) && GetSpeed() <= SOLID_SURFACE_COLLISION_REF) /*|| (m_characterData.m_buoyancyController.IsUnderwater() && collision.gameObject.tag == "Boat")*/)
            {
                Debug.Log("Jump");
                SetJumpInput();
                m_movement = Vector3.zero;
            }
        }
        m_isDolphinJump = false;
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
        RaycastHit objectHit;
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
            if (m_collisions.Count == 0 && !m_characterData.m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out objectHit, m_characterData.m_characterSO.m_rayCollisionRef))
            {
                m_characterData.m_characterSO.SetGroundedValue(false);
            }
        }
    }

    // TODO: same code as above? factorize?
    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        RaycastHit objectHit; // never used, make it local?
        if (m_collisions.Count == 0 && !m_characterData.m_characterSO.IsUnderWater() && !Physics.Raycast(transform.position, Vector3.down, out objectHit, m_characterData.m_characterSO.m_rayCollisionRef))
        {
            m_characterData.m_characterSO.SetGroundedValue(false);
        }
    }

    private void FixedUpdate()
    {
        // only the local player should process input
        if (!isLocalPlayer)
            return;

        // Just follow the boat if sailing
        if (State == CharacterControlState.Sailing)
        {
            transform.position = m_boatSeatTransform.position;
            transform.rotation = m_boatSeatTransform.rotation;

            return;
        }

        // Character is walking / jumping or swimming
        if (m_characterData.m_characterSO != null) // TODO: is this test still necessary?
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

    [ClientCallback]
    public void SetJumpInput()
    {
        if (!isLocalPlayer)
            return;
        m_triggerJump = true;
    }

    public void JumpingAndLanding()
    {
        if (IsJumpCoolDownOver() && m_characterData.m_characterSO.GetJumpInput() && m_characterData.m_buoyancyController != null && (m_characterData.m_characterSO.IsGrounded() || m_characterData.m_characterSO.IsUnderWater()))
        {
           m_jumpTimeStamp = Time.time;
            if(!m_characterData.m_buoyancyController.IsUnderwater())
            {
                m_rigidBody.AddForce(Vector3.up * m_characterData.m_characterSO.m_jumpForce, ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Water jump");
                m_isDolphinJump = true;
                m_rigidBody.AddForce(((Vector3.up * m_characterData.m_characterSO.m_underwaterJumpForce) + m_moveVector), ForceMode.Impulse);
            }
            m_characterData.m_characterSO.SetGroundedValue(false);
            m_characterData.m_characterSO.SetJumpInput(false);
        }

        if (m_animator && !m_wasGrounded && m_characterData.m_characterSO.IsGrounded())
        {
            m_joystickOffset = MAX_JOYSTICK_OFFSET;
            m_capsuleCollider.enabled = true;
            m_animator.SetTrigger("Land");
            StartCoroutine(ResetJumpTrigger());
        }

        if (m_animator && !m_characterData.m_characterSO.IsGrounded() && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
            m_joystickOffset = MIN_JOYSTICK_OFFSET;
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
        if (m_characterData.m_characterSO.m_isUnderwater)
            m_moveSpeed = m_characterData.m_characterSO.m_swimSpeed;
        else
            m_moveSpeed = m_characterData.m_characterSO.m_runSpeed;
        if(m_characterData.m_characterSO.m_jumpInput)
        {
            dir.x = -Mathf.Clamp(m_joystick.m_horizontal, -m_joystickOffset, Mathf.Sign(m_joystick.m_horizontal) * m_joystickOffset);
            dir.z = -Mathf.Clamp(m_joystick.m_vertical, -m_joystickOffset, Mathf.Sign(m_joystick.m_vertical) * m_joystickOffset);
        }
        else
        {
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

    public float GetSpeed()
    {
        float speedPerSec = Vector3.Distance(m_oldPosition, transform.position) / Time.deltaTime;
        m_oldPosition = transform.position;
        return speedPerSec;
    }

    public IEnumerator ResetJumpTrigger()
    {
        float delay = 1.5f;
        yield return new WaitForSeconds(delay);
        m_animator.ResetTrigger("Land");
        m_animator.ResetTrigger("Jump");
    }

    public IEnumerator GeneratePlayerHeadObj()
    {
        float delay = 0.3f;
        yield return new WaitForSeconds(delay);
        GameObject head = new GameObject("Head");
        Transform parent = m_characterData.m_fishingRodController.transform.Find(m_characterData.m_characterSO.m_headParentPath);
        head.transform.SetParent(parent);
        head.transform.localPosition = m_characterData.m_characterSO.m_headLocalPos;
        head.transform.localEulerAngles = m_characterData.m_characterSO.m_headLocalRot;
        m_playerHeadObj = head;
    }
}
