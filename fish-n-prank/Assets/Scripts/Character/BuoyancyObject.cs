using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    public CharacterSO m_characterSO;
    public BoatSO m_boatSO;
    public Transform[] m_floaters;

    Rigidbody m_rigidBody;
    int m_floatersUnderWater;

    public float m_waterHeight;
    public float m_floatingPower;
    public float m_underWaterDragForce;
    public float m_underWaterAngularDragForce;
    public float m_airDragForce;
    public float m_airAngularDrag;
    private const float BACKSTROKE_SWIM_ROT = 180f;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_boatSO != null)
            Init(m_boatSO.m_floatingPower, m_boatSO.m_waterHeight, m_boatSO.m_underWaterDragForce, m_boatSO.m_underWaterAngularDragForce,
                m_boatSO.m_airDragForce, m_boatSO.m_airAngularDrag);
    }

    private void FixedUpdate()
    {
        if(m_characterSO != null || m_boatSO != null)
        {
            m_floatersUnderWater = 0;
            for (int i = 0; i < m_floaters.Length; i++)
            {
                float difference = m_floaters[i].position.y - m_waterHeight;
                if (difference < 0)
                {
                    m_rigidBody.AddForceAtPosition(Vector3.up * m_floatingPower * Mathf.Abs(difference), m_floaters[i].position, ForceMode.Force);
                    m_floatersUnderWater += 1;
                    if (m_characterSO != null && !m_characterSO.IsUnderWater())
                    {
                        m_characterSO.SetUnderWaterValue(true);
                        m_characterSO.SetGroundedValue(true);
                        if (gameObject.tag == "Player")
                        {
                            GetComponent<Animator>().SetBool("Swim", true);
                            if (m_characterSO.m_isBackstrokeSwim)
                                transform.GetChild(0).localEulerAngles = new Vector3(transform.GetChild(0).localEulerAngles.x, BACKSTROKE_SWIM_ROT, 0f);
                        }
                        SwitchState(true);
                    }
                    else if(difference > 0 && m_boatSO != null)
                        SwitchState(true);
                }
                if (m_characterSO != null && m_characterSO.IsUnderWater() && m_floatersUnderWater == 0)
                {
                    if (gameObject.tag == "Player")
                    {
                        GetComponent<Animator>().SetBool("Swim", false);
                        if (m_characterSO.m_isBackstrokeSwim)
                            transform.GetChild(0).localEulerAngles = new Vector3(transform.GetChild(0).localEulerAngles.x, 0f, 0f);
                    }
                    m_characterSO.SetUnderWaterValue(false);
                    SwitchState(false);
                }
                else if(m_boatSO != null && m_floatersUnderWater == 0)
                    SwitchState(false);
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
        return m_characterSO.IsUnderWater();
    }

    void SwitchState(bool _isUnderWater)
    {
        if(_isUnderWater)
        {
            m_rigidBody.drag = m_underWaterDragForce;
            m_rigidBody.angularDrag = m_underWaterAngularDragForce;
            GameStateManager.CharactersManager.m_toggleFishingRodEvent.Raise();
        }
        else
        {
            m_rigidBody.drag = m_airDragForce;
            m_rigidBody.angularDrag = m_airAngularDrag;
        }
    }
}
