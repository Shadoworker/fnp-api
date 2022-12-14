using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    private const float MAX_RAY_DISTANCE = 4f;
    public CharacterData m_characterData;
    public BoatSO m_boatSO;
    public Transform[] m_floaters;
    public LayerMask m_waterLayer;
    Rigidbody m_rigidBody;
    int m_floatersUnderWater;

    private float m_waterHeight;
    private float m_floatingPower;
    private float m_underWaterDragForce;
    private float m_underWaterAngularDragForce;
    private float m_airDragForce;
    private float m_airAngularDrag;
    private const float BACKSTROKE_SWIM_ROT = 180f;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_boatSO != null)   
        {
            Init(m_boatSO.m_floatingPower, m_boatSO.m_waterHeight, m_boatSO.m_underWaterDragForce, m_boatSO.m_underWaterAngularDragForce,
                m_boatSO.m_airDragForce, m_boatSO.m_airAngularDrag);
        }else
        {
            m_characterData = GetComponent<CharacterData>();
        }
    }

    private void FixedUpdate()
    {
        if(m_characterData != null || m_boatSO != null)
        {
            m_floatersUnderWater = 0;
            AutoJump();
            for (int i = 0; i < m_floaters.Length; i++)
            {
                float difference = m_floaters[i].position.y - m_waterHeight;
                if (difference < 0)
                {
                    m_rigidBody.AddForceAtPosition(Vector3.up * m_floatingPower * Mathf.Abs(difference), m_floaters[i].position, ForceMode.Force);
                    m_floatersUnderWater += 1;
                    if (m_characterData != null && !m_characterData.m_characterSO.IsUnderWater())
                    {
                        m_characterData.m_characterSO.SetUnderWaterValue(true);
                        m_characterData.m_characterSO.SetGroundedValue(true);
                        if (gameObject.tag == "Player")
                        {
                            m_characterData.m_animator.SetBool("Swim", true);
                            if (m_characterData.m_characterSO.m_isBackstrokeSwim)
                                m_characterData.m_characterSkin.transform.localEulerAngles = new Vector3(transform.GetChild(0).localEulerAngles.x, BACKSTROKE_SWIM_ROT, 0f);
                        }
                        SwitchState(true);
                    }
                    else if(difference > 0 && m_boatSO != null)
                        SwitchState(true);
                }
                if (m_characterData != null && m_characterData.m_characterSO.IsUnderWater() && m_floatersUnderWater == 0)
                {
                    if (gameObject.tag == "Player")
                    {
                        m_characterData.m_animator.SetBool("Swim", false);
                        m_characterData.m_characterSO.SetGroundedValue(true);
                        if (m_characterData.m_characterSO.m_isBackstrokeSwim)
                            m_characterData.m_characterSkin.transform.localEulerAngles = new Vector3(transform.GetChild(0).localEulerAngles.x, 0f, 0f);
                    }
                    m_characterData.m_characterSO.SetUnderWaterValue(false);
                    SwitchState(false);
                }
                else if(m_boatSO != null && m_floatersUnderWater == 0)
                    SwitchState(false);
            }
        }
    }

    public void AutoJump()
    {
        if (m_characterData != null && m_characterData.m_characterSO.IsUnderWater() && m_characterData.m_characterController.m_playerHeadObj != null)
        {
            RaycastHit objectHit;
            Vector3 fwd = m_characterData.m_characterController.m_playerHeadObj.transform.TransformDirection(m_characterData.m_characterSO.m_boatDetectionRay);
            //Debug.DrawRay(m_characterData.m_characterController.m_playerHeadObj.transform.position, fwd * m_characterData.m_characterSO.m_maxRayDistance, Color.red);
            if (Physics.Raycast(m_characterData.m_characterController.m_playerHeadObj.transform.position, fwd, out objectHit, m_characterData.m_characterSO.m_maxRayDistance, ~m_waterLayer) && !m_characterData.m_characterController.m_triggerJump)
            {
                if (objectHit.transform.gameObject.tag == "Boat")
                {
                    m_characterData.m_characterController.SetJumpInput();
                    m_characterData.m_characterController.m_moveVector = Vector3.zero;
                }
            }
        }
    }

    public void Init(float _floatingPower, float _waterHeight,float _underWaterDragForce,float _underWaterAngularDragForce, 
        float _airDragForce, float _airAngularDrag)
    {
        m_floatingPower = _floatingPower;
        m_waterHeight = _waterHeight;
        m_underWaterAngularDragForce = _underWaterAngularDragForce;
        m_underWaterDragForce = _underWaterDragForce;
        m_airDragForce = _airDragForce;
        m_airAngularDrag = _airAngularDrag;
    }

    public bool IsUnderwater()
    {
        return m_characterData.m_characterSO.IsUnderWater();
    }

    void SwitchState(bool _isUnderWater)
    {
        if(_isUnderWater)
        {
            m_rigidBody.drag = m_underWaterDragForce;
            m_rigidBody.angularDrag = m_underWaterAngularDragForce;
            if(m_characterData != null)
            {
                m_characterData.m_fishingRodController.DeactivateFishingRod();
                m_characterData.m_characterController.m_capsuleCollider.direction = 2;
            }
        }
        else
        {
            m_rigidBody.drag = m_airDragForce;
            m_rigidBody.angularDrag = m_airAngularDrag;
            if (m_characterData != null)
                m_characterData.m_characterController.m_capsuleCollider.direction = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.tag == "Water" && m_characterData != null && m_characterData.m_characterSO.GetJumpInput())
        {
            m_characterData.m_animator.SetBool("Swim", true);
            m_characterData.m_characterSO.SetGroundedValue(true);
            m_characterData.m_characterController.m_triggerJump = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water" && m_characterData != null)
        {
            m_characterData.m_characterSO.SetGroundedValue(false);
            m_characterData.m_animator.SetBool("Swim", false);
        }

    }
}
