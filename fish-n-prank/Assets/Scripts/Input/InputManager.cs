using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    VariableJoystick m_movementJoystick;
    public VariableJoystick MovementJoystick { get { return m_movementJoystick; } }

    [SerializeField]
    VariableJoystick m_cameraJoystick;
    public VariableJoystick CameraJoystick { get { return m_cameraJoystick; } }
}
