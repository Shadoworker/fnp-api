using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    public Transform[] m_floaters;
    public float m_underWaterDrag = 3;
    public float m_underWaterAngularDrag = 1;

    public float m_airDrag = 0f;
    public float m_airAngularDrag = 0.05f;
    public float m_floatingPower = 15f;
    Rigidbody m_rigidBody;
    public float m_waterHeight;
    public bool m_underWater;
    int m_floatersUnderWater;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_floatersUnderWater = 0;
        for (int i = 0; i < m_floaters.Length; i++)
        {
            float difference = m_floaters[i].position.y - m_waterHeight;
            if (difference < 0)
            {
                m_rigidBody.AddForceAtPosition(Vector3.up * m_floatingPower * Mathf.Abs(difference), m_floaters[i].position, ForceMode.Force);
                m_floatersUnderWater += 1;
                if (!m_underWater)
                {
                    m_underWater = true;
                    if(gameObject.tag == "Player")
                        GetComponent<Animator>().SetBool("Swim", true);
                    SwitchState(true);
                }
            }
            if (m_underWater && m_floatersUnderWater == 0)
            {
                if (gameObject.tag == "Player")
                    GetComponent<Animator>().SetBool("Swim", false);
                m_underWater = false;
                SwitchState(false);
            }
        }
    }

    public bool IsUnderwater()
    {
        return m_underWater;
    }

    void SwitchState(bool _isUnderWater)
    {
        if(_isUnderWater)
        {
            m_rigidBody.drag = m_underWaterDrag;
            m_rigidBody.angularDrag = m_underWaterAngularDrag;
        }
        else
        {
            m_rigidBody.drag = m_airDrag;
            m_rigidBody.angularDrag = m_airAngularDrag;
        }
    }
}
