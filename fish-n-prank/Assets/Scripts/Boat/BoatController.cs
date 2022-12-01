using UnityEngine;
using Mirror;

public class BoatController : NetworkBehaviour
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
    protected Quaternion StartRotation; // TODO: naming!
    protected ParticleSystem ParticleSystem; // TODO: naming!
    [SerializeField]
    [Tooltip("An hideable mesh for the boat, to unblock the view of the local player only.")]
    private GameObject m_hideableMesh = null;

    //internal Properties
    protected Vector3 CamVel; // TODO: naming!
    private VariableJoystick m_joystick = null;
    public Transform m_facingDirection;
    private Vector3 m_moveVector { set; get; } // TODO: unused
    [SerializeField]
    private DriveBoatTrigger m_driveBoatTrigger;

    GameObject m_boatOwner = null;

    public void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        m_rigidbody = GetComponent<Rigidbody>();
        StartRotation = m_motor.localRotation;
        m_joystick = GameObject.Find("JoystickContainer").GetComponent<VariableJoystick>(); // TODO: better get from NetworkPlayer.LocalPlayer
    }

    #region SERVER
    [Command(requiresAuthority = false)]
    public void CmdStartSailing(GameObject _newBoatOwner)
    {
        // Deny if some other player is sailing
        if (m_boatOwner != null)
        {
            Debug.Log($"Player {_newBoatOwner} can't sail this boat, player {m_boatOwner} is already sailing it.");
            return;
        }

        m_boatOwner = _newBoatOwner;
        netIdentity.AssignClientAuthority(m_boatOwner.GetComponent<NetworkIdentity>().connectionToClient);

        Debug.Log($"Boat {gameObject.name} authority transfered to client {connectionToClient}");
        DoStartSailing();
    }

    [Command(requiresAuthority = false)]
    public void CmdStopSailing(GameObject _requesterPlayer)
    {
        if (m_boatOwner != _requesterPlayer)
        {
            Debug.LogWarning($"Player {_requesterPlayer} is trying to release bot owned by player {m_boatOwner}.");
            return;
        }

        DoStopSailing();

        m_boatOwner = null;
        netIdentity.RemoveClientAuthority();

        Debug.Log($"Boat {gameObject.name} authority transfered back to server");
    }
    #endregion

    #region CLIENT
    public void RequestStartSailing()
    {
        // TODO: improve that test
        if (GameStateManager.CharactersManager.LocalPlayer.GetComponent<CharacterController>().State == CharacterController.CharacterControlState.Walking)
        {
            CmdStartSailing(GameStateManager.CharactersManager.LocalPlayer);
        }
    }

    [TargetRpc]
    public void DoStartSailing()
    {
        m_driveBoatTrigger.OnStartSailing();
        if (m_hideableMesh != null)
        {
            m_hideableMesh.SetActive(false);
        }
    }

    public void RequestStopSailing()
    {
        // TODO: improve that test
        if (GameStateManager.CharactersManager.LocalPlayer.GetComponent<CharacterController>().State == CharacterController.CharacterControlState.Sailing)
        {
            CmdStopSailing(GameStateManager.CharactersManager.LocalPlayer);
        }
    }

    [TargetRpc]
    public void DoStopSailing()
    {
        m_driveBoatTrigger.OnStopSailing();
        if (m_hideableMesh != null)
        {
            m_hideableMesh.SetActive(true);
        }
    }

    public void FixedUpdate()
    {
        // TODO: check that local character is sailing

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
    #endregion
}
