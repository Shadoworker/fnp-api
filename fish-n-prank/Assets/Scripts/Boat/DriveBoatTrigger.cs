using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DriveBoatTrigger : MonoBehaviour
{
    private CharacterController m_character;
    public BoatController m_boat;
    public GameObject m_driveBtn;
    public Transform m_playerSeatPos;
    private const string NAVIGATE_ANIM_PARAM = "Navigate";

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !m_boat.enabled) // TODO: use a constant for Tags
        {
            m_driveBtn.SetActive(true);
            m_character = other.GetComponentInParent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && m_character.enabled)
        {
            m_driveBtn.SetActive(false);
        }
    }

    public void SailBoat()
    {
        GameStateManager.CameraManager.m_isCameraCentered = false;
        m_character.transform.LookAt(m_boat.m_facingDirection);
        m_character.transform.SetParent(m_boat.transform);
        m_character.transform.localPosition = new Vector3(m_playerSeatPos.localPosition.x, m_playerSeatPos.transform.localPosition.y, m_playerSeatPos.localPosition.z);
        m_character.transform.rotation = m_playerSeatPos.rotation;
        m_character.m_characterData.m_animator.SetBool(NAVIGATE_ANIM_PARAM, true);
        GameStateManager.CameraManager.SetTarget(m_boat.gameObject, false, m_boat.transform);
        m_character.GetComponent<Rigidbody>().isKinematic = true;
        m_character.enabled = false;
        m_boat.enabled = true;
        m_driveBtn.SetActive(false);
    }

    public void LeaveBoat()
    {
        if(m_character != null && !m_character.enabled)
        {
            GameStateManager.CameraManager.SetTarget(m_character.gameObject);
            m_character.m_characterData.m_animator.SetBool(NAVIGATE_ANIM_PARAM, false);
            m_character.transform.SetParent(null);
            m_character.GetComponent<Rigidbody>().isKinematic = false;
            m_character.enabled = true;
            m_boat.enabled = false;
        }
    }
}
