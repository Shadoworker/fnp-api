using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingRodController : MonoBehaviour
{
    public CharacterData m_characterData;
    private GameObject m_fishingRod;
    private Transform m_playerHead;
    private CharacterController m_characterController;
    private Animator m_animator;
    RaycastHit objectHit;

    public void InitFishingRod()
    {
        m_playerHead = transform.Find(m_characterData.m_characterSO.m_playerHeadObjName);
        m_fishingRod = transform.Find(m_characterData.m_characterSO.m_fishingRodObjName).gameObject;
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (m_fishingRod.activeInHierarchy && !FishingController.Instance.m_isFishing 
            && !m_characterData.m_buoyancyController.IsUnderwater())
        {
            Vector3 fwd = m_playerHead.TransformDirection(new Vector3(1, 5f, 1.5f));
            Debug.DrawRay(m_playerHead.position, fwd, Color.red);
            if (Physics.Raycast(m_playerHead.position, fwd, out objectHit, 40))
            {
                if (objectHit.transform.gameObject.name == "Water")
                {
                    FishingController.Instance.m_characterController = m_characterController;
                    FishingController.Instance.m_isNearFishingSpot.Raise("true");
                }
                else
                {
                    FishingController.Instance.m_isNearFishingSpot.Raise("false");
                }
            }
        }
        else
        {
            if (FishingController.Instance.m_isNearFishingSpot.m_previousValue.Equals("true"))
                FishingController.Instance.m_isNearFishingSpot.Raise("false");
        }
    }

    public Transform GetPlayerHead()
    {
        return m_playerHead;
    }
    public void ToggleFishingRod()
    {
        if(!m_fishingRod.activeInHierarchy && !m_characterData.m_characterSO.IsUnderWater())
        {
            m_animator.SetTrigger("GrabRod");
            m_fishingRod.SetActive(true);
        }
        else
        {
            m_animator.SetTrigger("DropRod");
            m_fishingRod.SetActive(false);
        }
    }

    public void DeactivateFishingRod()
    {
        m_fishingRod.SetActive(false);
    }
}
