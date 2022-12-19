using UnityEngine;
using TMPro;
using PathCreation;
using Kayfo;

public class FishingManager : Singleton<FishingManager>
{
    private const float MIN_SAFE_ZONE = 15837;
    private const float MAX_SAFE_ZONE = 73594;
    private const float INDICATOR_DEFAULT_POS = 43536;

    public PathCreator m_pathCreator;
    public Transform m_indicator;
    public float m_pullingPower;
    public float m_speed = 5;
    public float m_battleDuration;
    public bool m_isFishing;
    float m_distanceTravelled;
    public TextMeshProUGUI m_numberOfCaughtFishesText;
    public GameObject m_fishingUI, m_fishBtn, m_jumpBtn;
    public StringEvent m_isNearFishingSpot;
    public CharacterController m_characterController;
    public Vector3 m_fishingUILocalPos;

    private PersistentInt m_caughtFishes = new PersistentInt("CAUGHT_FISH", 0);

    private void Start()
    {
        RefeshCaughtFishesUI();
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
                m_caughtFishes.Set(m_caughtFishes.Get() + 1);
                RefeshCaughtFishesUI();
                EndFishing();
            }
        }
    }

    private void RefeshCaughtFishesUI()
    {
        m_numberOfCaughtFishesText.text = m_caughtFishes.Get().ToString();
    }

    private void FixedUpdate()
    {
        m_indicator.position = m_pathCreator.path.GetPointAtDistance(m_distanceTravelled);
    }

    public void VerifiedFishingState()
    {
        if (m_isNearFishingSpot.m_previousValue.Equals("true"))
            m_fishBtn.SetActive(true);
        else
            m_fishBtn.SetActive(false);
    }
    public void EndFishing()
    {
        m_fishingUI.SetActive(false);
        m_jumpBtn.SetActive(true);
        m_isFishing = false;
        if (GameStateManager.CharactersManager.LocalPlayer != null) // TODO: test still necessary?
        {
            CharacterController characterController = GameStateManager.CharactersManager.LocalPlayer.GetComponent<CharacterController>();
            characterController.enabled = true;
            characterController.m_rigidBody.isKinematic = false;
            characterController.m_animator.SetBool("Fish", false);
        }
    }


    /// <summary>
    /// Initiate server request to start fishing gameplay
    /// </summary>
    public void RequestStartFishingGameplay()
    {
        //GameStateManager.CharactersManager.LocalPlayer.GetComponent<FnPFishingNetworkInterface>().CmdStartFishing(gameObject);
        GameStateManager.CharactersManager.LocalPlayer.GetComponent<FnPFishingNetworkInterface>().RequestStartFishing();
    }

    /// <summary>
    /// Starts fishing gameplay locally (server validation done)
    /// </summary>
    /// <param name="_fishSO">The fish that just bit</param>
    public void StartFishingGameplay(SelectedFish _fishSO)
    {
        Debug.Log($"ShouldStartFishingGameplay: received fish from Server: {_fishSO.name}");

        // initialize rode settings
        m_isFishing = true;
        m_distanceTravelled = INDICATOR_DEFAULT_POS;
        m_speed = _fishSO.m_speed;
        m_battleDuration = _fishSO.m_battleDuration;
        m_isNearFishingSpot.Raise("false");

        // setup character
        CharacterController characterController = GameStateManager.CharactersManager.LocalPlayer.GetComponent<CharacterController>();
        characterController.enabled = false;
        characterController.m_rigidBody.isKinematic = true;
        characterController.m_animator.SetBool("Fish", true);
        characterController.m_characterData.m_fishingRodController.ToggleFishingRod();

        // refresh UI
        // TODO: extract those commands to UIManager.StartFishingGameplay()
        m_fishingUI.SetActive(true);
        m_jumpBtn.SetActive(false);
        m_fishBtn.SetActive(false);
    }
}
