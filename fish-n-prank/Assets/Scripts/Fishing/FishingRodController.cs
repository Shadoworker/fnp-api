using UnityEngine;
using System.Collections;

public class FishingRodController : MonoBehaviour
{
    private const float MAX_RAY_DISTANCE = 40f;
    private const float THROW_FORCE = 30f;
    public CharacterData m_characterData;
    private GameObject m_fishingRod;
    [HideInInspector]public FishingHook m_fishingHook;
    private Transform m_playerHead;
    private CharacterController m_characterController;
    private Animator m_animator;
    private bool m_isOnFishingSpot;
    RaycastHit objectHit;

    private bool m_initialized = false;

    public void InitFishingRod()
    {
        m_playerHead = transform.Find(m_characterData.m_characterSO.m_playerHeadObjName);
        m_fishingRod = transform.Find(m_characterData.m_characterSO.m_fishingRodObjName).gameObject;
        m_fishingHook = m_fishingRod.GetComponent<CharacterHandTracker>().m_fishingRodHook.GetComponent<FishingHook>();
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_initialized = true;
    }

    public void Update()
    {
        if (!m_initialized) return;

        if (!transform.parent.GetComponent<NetworkPlayer>().isLocalPlayer)
            return;

        if (!FishingManager.Instance.m_isFishing
            && !m_characterData.m_buoyancyController.IsUnderwater() && m_characterData.m_characterController.m_playerHeadObj != null) // TODO: do you mean IsSwimming?
        {
            Vector3 fwd = m_characterData.m_characterController.m_playerHeadObj.transform.TransformDirection(m_characterData.m_characterSO.m_fishingRay);
            Debug.DrawRay(m_characterData.m_characterController.m_playerHeadObj.transform.position, fwd, Color.red);
            if (Physics.Raycast(m_characterData.m_characterController.m_playerHeadObj.transform.position, fwd, out objectHit, MAX_RAY_DISTANCE))
            {
                if (objectHit.transform.gameObject.name == "Water") // TODO: use const
                {
                    FishingManager.Instance.m_characterController = m_characterController;
                    FishingManager.Instance.m_isNearFishingSpot.Raise("true");
                    m_isOnFishingSpot = true;
                }
                else
                {
                    FishingManager.Instance.m_isNearFishingSpot.Raise("false");
                    m_isOnFishingSpot = false;
                    if (m_fishingRod.activeInHierarchy)
                        ToggleFishingRod();
                }
            }
        }
        else
        {
            if (FishingManager.Instance.m_isNearFishingSpot.m_previousValue.Equals("true"))
                FishingManager.Instance.m_isNearFishingSpot.Raise("false");
        }
    }

    public void ThrowFishingHook()
    {
        m_fishingHook.GetComponent<Rigidbody>().AddForce(transform.forward * THROW_FORCE, ForceMode.Impulse);
        StartCoroutine(SetHookPos());
    }

    IEnumerator SetHookPos()
    {
        yield return new WaitForSeconds(0.2f);
        m_fishingHook.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ToggleFishingRod()
    {
        if (!transform.parent.GetComponent<NetworkPlayer>().isLocalPlayer)
            return;

        if(!m_fishingRod.activeInHierarchy && !m_characterData.m_characterSO.IsUnderWater())
        {
            transform.parent.GetComponent<NetworkPlayer>().SetFishingRodEnabled(true);
        }
        else if(!FishingManager.Instance.m_isFishing)
        {
            transform.parent.GetComponent<NetworkPlayer>().SetFishingRodEnabled(false);
        }
    }

    public void OnFishingRodStateChange(bool _fishingRodEnabled)
    {
        if ((!m_fishingRod.activeInHierarchy && !m_characterData.m_characterSO.IsUnderWater() && m_isOnFishingSpot) || (_fishingRodEnabled && !m_fishingRod.activeInHierarchy))
        {
            if (m_animator) m_animator.SetTrigger("GrabRod"); // TODO: const + fix animator not set
            m_fishingRod.SetActive(true);
        }
        else if((m_isOnFishingSpot || m_fishingRod.activeInHierarchy) && !_fishingRodEnabled)
        {
            if (m_animator) m_animator.SetTrigger("DropRod"); // TODO: const + fix animator not set
            m_fishingRod.SetActive(false);
        }
    }

    public void DeactivateFishingRod()
    {
        m_fishingRod.SetActive(false);
    }
}
