using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;
using TMPro;


public class FishingRodController : MonoBehaviour
{
    private const float MIN_SAFE_ZONE = 15837;
    private const float MAX_SAFE_ZONE = 73594;
    private const float INDICATOR_DEFAULT_POS = 43536;
    private const string NUMBER_OF_CAUGHT_FISHES = "CAUGHT_FISH";
    public GameObject m_fishingRod;
    public PathCreator m_pathCreator;
    public float m_pullingPower;
    public float m_speed = 5;
    public float m_batteleDuration;
    float m_distanceTravelled;
    public Transform m_indicator;
    public List<FishType> m_fishTypes;
    public bool m_isFishing;
    public GameObject m_fishingUI, m_fishBtn, m_jumpBtn, m_runBtn;
    public Transform m_playerHead;
    public TextMeshProUGUI m_numberOfCaughtFishesText;
    private void Start()
    {
        InitRodValues();
        m_numberOfCaughtFishesText.text = PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES).ToString();
    }

    RaycastHit objectHit;
    public void Update()
    {
        if (m_fishingRod.activeInHierarchy && !m_fishingUI.activeInHierarchy)
        {
            Vector3 fwd = m_playerHead.TransformDirection(new Vector3(1, 5f, 1.5f));
            Debug.DrawRay(m_playerHead.position, fwd, Color.green);
            if (Physics.Raycast(m_playerHead.position, fwd, out objectHit, 40))
            {
                if (objectHit.transform.gameObject.name == "Water_plane")
                {
                    m_fishBtn.SetActive(true);
                }
                else
                {
                    m_fishBtn.SetActive(false);
                }
            }
        }
        else
        {
            if(m_fishBtn.activeInHierarchy)
                m_fishBtn.SetActive(false);
        }

        if (m_fishingRod.activeInHierarchy && m_isFishing)
        {
            m_batteleDuration -= Time.deltaTime;
            int fishResistance = Random.Range(0, 2);
            if (fishResistance == 1)
                m_distanceTravelled -= m_speed * Time.deltaTime;
            if (Input.GetMouseButtonUp(0))
            {
                m_distanceTravelled += m_pullingPower * Time.deltaTime;
            }

            if (m_distanceTravelled < MIN_SAFE_ZONE || m_distanceTravelled > MAX_SAFE_ZONE)
                EndFishing();
            if (m_batteleDuration <= 0)
            {
                PlayerPrefs.SetInt(NUMBER_OF_CAUGHT_FISHES, PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES) + 1);
                m_numberOfCaughtFishesText.text = PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES).ToString();
                EndFishing();
            }
            m_indicator.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled);
        }
    }

    public void EndFishing()
    {
        m_fishingUI.SetActive(false);
        m_jumpBtn.SetActive(true);
        m_runBtn.SetActive(true);
        m_isFishing = false;
        GetComponent<CharacterController>().enabled = true;
    }
    public void ToggleFishingRod(GameObject _btn)
    {
        if(!m_fishingRod.activeInHierarchy)
        {
            m_fishingRod.SetActive(true);
            _btn.GetComponent<Image>().color = Color.green;
        }
        else
        {
            m_fishingRod.SetActive(false);
            _btn.GetComponent<Image>().color = Color.white;
        }
    }

    public void ActivateFishingGameplay(GameObject _btn)
    {
        InitRodValues();
        GetComponent<CharacterController>().enabled = false;
        m_isFishing = true;
        m_fishingUI.SetActive(true);
        m_jumpBtn.SetActive(false);
        m_runBtn.SetActive(false);
        _btn.SetActive(false);
    }

    public void InitRodValues()
    {
        FishType fish = m_fishTypes[Random.Range(0, m_fishTypes.Count)];
        m_distanceTravelled = INDICATOR_DEFAULT_POS;
        m_speed = fish.m_speed;
        m_batteleDuration = fish.m_battleDuration;
    }
}

public enum FISH_TYPE
{
    WEAK,
    MID,
    STRONG
}

[System.Serializable]
public class FishType
{
    public float m_battleDuration;
    public float m_speed;
}