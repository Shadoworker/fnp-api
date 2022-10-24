using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(FishingRodController), typeof(CharacterController), typeof(GameEventListener))]
public class CharacterData : MonoBehaviour
{
    public CharacterSO m_characterSO;

    private void Start()
    {
        AddCharacterControllerScript();
        AddBuoyancyScript();
        AddFishingRodController();
    }
    public void AddCharacterControllerScript()
    {
        gameObject.GetComponent<CharacterController>().m_characterSO = m_characterSO;
        gameObject.GetComponent<CharacterController>().InitCharacterControllerValues();
        gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void AddBuoyancyScript()
    {
        gameObject.AddComponent<BuoyancyObject>();
        gameObject.GetComponent<BuoyancyObject>().m_characterSO = m_characterSO;
        gameObject.GetComponent<BuoyancyObject>().m_floaters = new Transform[1];
        gameObject.GetComponent<BuoyancyObject>().m_floaters[0] = transform;
        gameObject.GetComponent<BuoyancyObject>().Init(m_characterSO.m_floatingPower, m_characterSO.m_waterHeight,
            m_characterSO.m_underWaterDragForce, m_characterSO.m_underWaterAngularDragForce, m_characterSO.m_airDragForce, m_characterSO.m_airAngularDrag);
    }

    public void AddFishingRodController()
    {
        gameObject.GetComponent<FishingRodController>().m_characterSO = m_characterSO;
        gameObject.GetComponent<FishingRodController>().InitFishingRod();
    }
}
