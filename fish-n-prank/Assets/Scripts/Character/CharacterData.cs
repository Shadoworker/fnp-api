using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

[RequireComponent(typeof(CharacterController), typeof(GameEventListener))]
public class CharacterData : NetworkBehaviour
{
    public CharacterSO m_characterSO;
    public CharacterController m_characterController;
    public BuoyancyObject m_buoyancyController;
    public FishingRodController m_fishingRodController;
    public Animator m_animator;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameStateManager.CharactersManager.LocalPlayer = NetworkClient.localPlayer.gameObject;
        GameStateManager.CameraManager.SetTarget(GameStateManager.CharactersManager.LocalPlayer);
    }

    public void InitCharacterData(CharacterSO _characterSO)
    {
        m_characterSO = _characterSO;
        AddBuoyancyScript();
        InitCharacterController();
        AddFishingRodController();
        m_animator.runtimeAnimatorController = m_characterSO.m_animator;
    }

    public void InitCharacterController()
    {
        m_characterController = GetComponent<CharacterController>();
        m_characterController.m_characterData = this;
        m_characterController.m_animator = m_animator;
        m_characterController.InitCharacterControllerValues();
        m_characterController.enabled = true;
    }

    public void AddBuoyancyScript()
    {
        m_buoyancyController = GetComponent<BuoyancyObject>();
        m_buoyancyController.m_characterData = this;
        m_buoyancyController.m_floaters = new Transform[1];
        m_buoyancyController.m_floaters[0] = transform;
        m_buoyancyController.Init(m_characterSO.m_floatingPower, m_characterSO.m_waterHeight,
            m_characterSO.m_underWaterDragForce, m_characterSO.m_underWaterAngularDragForce, m_characterSO.m_airDragForce, m_characterSO.m_airAngularDrag);
        m_buoyancyController.enabled = true;
    }

    public void AddFishingRodController()
    {
        m_fishingRodController.m_characterData = GetComponent<CharacterData>();
        m_fishingRodController.InitFishingRod();
        m_fishingRodController.enabled = true;
    }
}
