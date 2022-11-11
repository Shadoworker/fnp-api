using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(FishingRodController), typeof(CharacterController), typeof(GameEventListener))]
public class CharacterData : MonoBehaviour
{
    public CharacterSO m_characterSO;
    public CharacterController m_characterController;
    public BuoyancyObject m_buoyancyController;
    public FishingRodController m_fishingRodController;

    private void Start()
    {
        AddBuoyancyScript();
        AddCharacterControllerScript();
        AddFishingRodController();
    }
    public void AddCharacterControllerScript()
    {
        m_characterController = GetComponent<CharacterController>();
        gameObject.GetComponent<CharacterController>().m_characterSO = m_characterSO;
        gameObject.GetComponent<CharacterController>().InitCharacterControllerValues();
        gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void AddBuoyancyScript()
    {
        gameObject.AddComponent<BuoyancyObject>();
        m_buoyancyController = GetComponent<BuoyancyObject>();
        gameObject.GetComponent<BuoyancyObject>().m_characterSO = m_characterSO;
        gameObject.GetComponent<BuoyancyObject>().m_floaters = new Transform[1];
        gameObject.GetComponent<BuoyancyObject>().m_floaters[0] = transform;
        gameObject.GetComponent<BuoyancyObject>().Init(m_characterSO.m_floatingPower, m_characterSO.m_waterHeight,
            m_characterSO.m_underWaterDragForce, m_characterSO.m_underWaterAngularDragForce, m_characterSO.m_airDragForce, m_characterSO.m_airAngularDrag);
    }

    public void AddFishingRodController()
    {
        m_fishingRodController = GetComponent<FishingRodController>();
        gameObject.GetComponent<FishingRodController>().m_characterSO = m_characterSO;
        gameObject.GetComponent<FishingRodController>().InitFishingRod();
    }
}
