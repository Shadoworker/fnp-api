using UnityEngine;

public class DriveBoatTrigger : MonoBehaviour // TODO: change script name
{
    private CharacterController m_character;
    public BoatController m_boat;
    public GameObject m_driveBtn;
    public Transform m_playerSeatPos;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != GameStateManager.CharactersManager.LocalPlayer)
        {
            return;
        }

        // check local player here
        if (other.tag == "Player" && !m_boat.enabled) // TODO: use a constant for Tags
        {
            m_driveBtn.SetActive(true);
            m_character = other.GetComponentInParent<CharacterController>(); // TODO: add test for local player here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != GameStateManager.CharactersManager.LocalPlayer)
        {
            return;
        }

        if (other.tag == "Player" && m_character.enabled)
        {
            m_driveBtn.SetActive(false);
        }
    }

    public void OnStartSailing()
    {
        GameStateManager.CameraManager.m_isCameraCentered = false;
        m_character.SetIsSailing(m_boat, m_playerSeatPos);
        m_boat.enabled = true;
        m_driveBtn.SetActive(false);
    }

    public void OnStopSailing()
    {
        m_character.SetIsWalking();
        m_character.enabled = true;
        m_boat.enabled = false;
    }
}
