using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathCreation;

public class FishingController : Singleton<FishingController>
{
    public PathCreator m_pathCreator;
    public Transform m_indicator;
    public List<FishType> m_fishTypes;
    public float m_pullingPower;
    public float m_speed = 5;
    public float m_battleDuration;
    public bool m_isFishing;
    private const float UI_HEIGHT = 200f;
    private const float MIN_SAFE_ZONE = 15837;
    private const float MAX_SAFE_ZONE = 73594;
    private const float INDICATOR_DEFAULT_POS = 43536;
    private const float HEIGHT_MARGIN = 100f;
    float m_distanceTravelled;
    public TextMeshProUGUI m_numberOfCaughtFishesText;
    public GameObject m_fishingUI, m_fishBtn, m_jumpBtn;
    private const string NUMBER_OF_CAUGHT_FISHES = "CAUGHT_FISH";
    public StringEvent m_isNearFishingSpot;
    public CharacterController m_characterController;
    public Vector3 m_fishingUILocalPos;

    private void Start()
    {
        InitRodValues();
        m_numberOfCaughtFishesText.text = PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES).ToString();
    }

    void Update()
    {
        if (m_isFishing)
        {
            m_battleDuration -= Time.deltaTime;
            int fishResistance = Random.Range(0, 2);
            if (fishResistance == 1)
                m_distanceTravelled -= m_speed * Time.deltaTime;
            if (Input.GetMouseButtonUp(0))
            {
                m_distanceTravelled += m_pullingPower * Time.deltaTime;
            }

            if (m_distanceTravelled < MIN_SAFE_ZONE || m_distanceTravelled > MAX_SAFE_ZONE)
                EndFishing();
            if (m_battleDuration <= 0)
            {
                PlayerPrefs.SetInt(NUMBER_OF_CAUGHT_FISHES, PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES) + 1);
                m_numberOfCaughtFishesText.text = PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES).ToString();
                EndFishing();
            }
            m_indicator.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled);
        }
    }

    public void InitRodValues()
    {
        FishType fish = m_fishTypes[Random.Range(0, m_fishTypes.Count)];
        m_distanceTravelled = INDICATOR_DEFAULT_POS;
        m_speed = fish.m_speed;
        m_battleDuration = fish.m_battleDuration;
        m_isNearFishingSpot.Raise("false");
    }

    public void VerifiedFishingState()
    {
        if(m_isNearFishingSpot.m_previousValue.Equals("true")) 
            m_fishBtn.SetActive(true);
        else
            m_fishBtn.SetActive(false);
    }
    public void EndFishing()
    {
        m_fishingUI.SetActive(false);
        m_jumpBtn.SetActive(true);
        m_isFishing = false;
        if(GameStateManager.CharactersManager.GetCurrentCharacter() != null)
        {
            CharacterController characterController = GameStateManager.CharactersManager.GetCurrentCharacter().GetComponent<CharacterController>();
            characterController.enabled = true;
            characterController.m_animator.SetBool("Fish", false);
        }
    }


    public void ActivateFishingGameplay()
    {
        InitRodValues();
        CharacterController characterController = GameStateManager.CharactersManager.GetCurrentCharacter().GetComponent<CharacterController>();
        characterController.enabled = false;
        m_isFishing = true;
        m_fishingUI.SetActive(true);
        m_jumpBtn.SetActive(false);
        m_fishBtn.SetActive(false);
        characterController.m_animator.SetBool("Fish", true);
        //SetFishingUIPosBasedOnPlayerPos();
    }

    public void SetFishingUIPosBasedOnPlayerPos()
    {
        CharacterController characterController = GameStateManager.CharactersManager.GetCurrentCharacter().GetComponent<CharacterController>();
        Transform characterHead = characterController.GetComponent<FishingRodController>().GetPlayerHead();
        m_fishingUI.transform.SetParent(characterHead.parent);
        m_fishingUI.transform.localPosition = new Vector3(characterHead.localRotation.x, characterHead.localPosition.y, characterHead.localPosition.z);
        m_fishingUI.transform.LookAt(characterController.transform);
        m_fishingUI.transform.localRotation = Quaternion.Euler(0, m_fishingUI.transform.rotation.y, m_fishingUI.transform.rotation.z);
        m_fishingUI.transform.SetParent(characterHead.transform);
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