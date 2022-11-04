using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DriveBoatTrigger : MonoBehaviour
{
    private const float CAMERA_SAILING_POS = -16f;
    private const float CAMERA_PLAYER_POS = -6f;
    private CharacterController m_character;
    public BoatController m_boat;
    public GameObject m_driveBtn;
    public Camera m_camera;
    public GameObject m_virtualCamera;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !m_boat.enabled)
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
        m_virtualCamera.gameObject.SetActive(true);
        m_camera.gameObject.SetActive(true);
        m_character.transform.localPosition = new Vector3(m_boat.transform.localPosition.x, m_character.transform.localPosition.y, m_character.transform.localPosition.z);
        m_character.transform.LookAt(m_boat.m_facingDirection);
        m_character.transform.SetParent(m_boat.transform);
        m_character.GetComponent<Rigidbody>().isKinematic = true;
        //m_character.GetComponent<CapsuleCollider>().enabled = false;
        m_character.enabled = false;
        m_boat.enabled = true;
        m_driveBtn.SetActive(false);
    }

    public void LeaveBoat()
    {
        if(m_character != null && !m_character.enabled)
        {
            m_virtualCamera.gameObject.SetActive(false);
            m_camera.gameObject.SetActive(false);
            m_character.transform.SetParent(null);
            m_character.GetComponent<Rigidbody>().isKinematic = false;
            //m_character.GetComponent<CapsuleCollider>().enabled = true;
            m_character.enabled = true;
            m_boat.enabled = false;
        }
    }
}
