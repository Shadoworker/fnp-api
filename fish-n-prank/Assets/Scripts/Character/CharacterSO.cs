using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum MovementMode
{
    CONSTANT,
    LINEAR
}

[CreateAssetMenu(menuName = "Configs/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    [FancyHeader("  Character SO  ", 1.5f, "lime", 5.5f, order = 0)]
    [Label("")] public Empty e;
    public CHARACTER m_character;
    [BoxGroup("Movement Controller")] private bool m_isRunning;
    [BoxGroup("Movement Controller")] public  MovementMode m_movementMode;
    [BoxGroup("Movement Controller")] public float m_runSpeed;
    [BoxGroup("Movement Controller")] public float m_walkSpeed;
    [BoxGroup("Movement Controller")] public float m_maxSpeed;
    [BoxGroup("Movement Controller")] public float m_rotationSpeed;
    [BoxGroup("Movement Controller")] public string m_playerHeadObjName;

    [BoxGroup("Animation Controller")] public float m_specialIdleRepeatRate;

    [BoxGroup("Jump Controller")] public float m_jumpForce;
    [BoxGroup("Jump Controller")] public float m_minJumpInterval;
    [BoxGroup("Jump Controller")] private bool m_jumpInput;
    [BoxGroup("Jump Controller")] private bool m_isGrounded;

    [BoxGroup("Physics")] public float m_dragForce;
    [BoxGroup("Physics")] public float m_mass;
    [BoxGroup("Physics")] public float m_underWaterDragForce;
    [BoxGroup("Physics")] public float m_underWaterAngularDragForce;
    [BoxGroup("Physics")] public float m_airDragForce;
    [BoxGroup("Physics")] public float m_airAngularDrag;

    [BoxGroup("Buoyancy")] public float m_floatingPower;
    [BoxGroup("Buoyancy")] public float m_diveForce;
    [BoxGroup("Buoyancy")] private bool m_isUnderwater;
    [BoxGroup("Buoyancy")] public float m_waterHeight;

    [BoxGroup("Fishing Controller")] public string m_fishingRodObjName;

    public bool GetJumpInput()
    {
        return m_jumpInput;
    }

    public void SetJumpInput(bool _value)
    {
        m_jumpInput = _value;
    }

    public bool IsGrounded()
    {
        return m_isGrounded;
    }

    public void SetGroundedValue(bool _value)
    {
        m_isGrounded = _value;
    }

    public bool IsRunning()
    {
        return m_isRunning;
    }

    public void SetRunningValue(bool _value)
    {
        m_isRunning = _value;
    }

    public bool IsUnderWater()
    {
        return m_isUnderwater;
    }

    public void SetUnderWaterValue(bool _value)
    {
        m_isUnderwater = _value;
    }
}


    [System.Serializable]
    public struct Empty
    {
    }


