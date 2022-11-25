using UnityEngine;
using TMPro;
using PathCreation;

public class FishingController : MonoBehaviour
{
    public PathCreator m_pathCreator;
    public Transform m_indicator;
    public float m_pullingPower;
    public float m_speed = 5;
    public float m_battleDuration;
    public bool m_isFishing;
    private const float MIN_SAFE_ZONE = 15837;
    private const float MAX_SAFE_ZONE = 73594;
    private const float INDICATOR_DEFAULT_POS = 43536;
    float m_distanceTravelled;
    public TextMeshProUGUI m_numberOfCaughtFishesText;
    public GameObject m_fishingUI, m_fishBtn, m_jumpBtn;
    private const string NUMBER_OF_CAUGHT_FISHES = "CAUGHT_FISH";
    public StringEvent m_isNearFishingSpot;
    public CharacterController m_characterController;
    public Vector3 m_fishingUILocalPos;

    #region Singleton
    public static FishingController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void Start()
    {
        // TODO: that data should come from a cloud save in the future
        m_numberOfCaughtFishesText.text = PlayerPrefs.GetInt(NUMBER_OF_CAUGHT_FISHES).ToString(); // TODO: use a PersistenInt
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
        }
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
    public void StartFishingGameplay(FishSO _fishSO)
    {
        Debug.Log($"ShouldStartFishingGameplay: received fish from Server: {_fishSO.name}");

        // initialize rode settings
        m_distanceTravelled = INDICATOR_DEFAULT_POS;
        m_speed = _fishSO.m_speed;
        m_battleDuration = _fishSO.m_battleDuration;
        m_isNearFishingSpot.Raise("false");

        // setup character
        CharacterController characterController = GameStateManager.CharactersManager.LocalPlayer.GetComponent<CharacterController>();
        characterController.enabled = false;
        characterController.m_animator.SetBool("Fish", true);

        // refresh UI
        // TODO: extract those commands to UIManager.StartFishingGameplay()
        m_fishingUI.SetActive(true);
        m_jumpBtn.SetActive(false);
        m_fishBtn.SetActive(false);

        m_isFishing = true;
    }
}