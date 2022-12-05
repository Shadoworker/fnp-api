using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnChangeSkin))]
    private string m_currentSkinName;
    private GameObject m_activeSkin = null;

    [SyncVar(hook = nameof(OnChangeFishingRodState))]
    private bool m_fishingRodEnabled = false;

    #region SERVER
    [Server]
    public void SetUseCharacter(string _characterName)
    {
        if (_characterName == CHARACTER.RANDOM.ToString())
        {
            m_currentSkinName = GameStateManager.CharactersManager.GetRandomSkin();
        }
        else
        {
            m_currentSkinName = _characterName;
        }

        CharacterSO characterSO = GameStateManager.CharactersManager.GetCharacterSO(m_currentSkinName);

        string lookupName = $"character-{characterSO.name.ToLower().Replace('_', '-')}";
        foreach (Transform characterSkin in transform)
        {
            if (characterSkin.gameObject.name.ToLower() == lookupName)
            {
                GetComponent<NetworkAnimator>().animator = characterSkin.gameObject.GetComponent<Animator>();
            }
        }

        // TODO: handle the skin not found case, with an error message
    }

    [Command]
    public void RequestChangeFishingRodState(bool _newState)
    {
        m_fishingRodEnabled = _newState;
    }
    #endregion


    #region CLIENT
    [Client]
    void OnChangeSkin(string _oldSkinName, string _newSkinName)
    {
        CharacterSO characterSO = GameStateManager.CharactersManager.GetCharacterSO(_newSkinName);

        string lookupName = $"character-{characterSO.name.ToLower().Replace('_', '-')}";
        GameObject characterGO = null;
        foreach (Transform characterSkin in transform)
        {
            if (characterSkin.gameObject.name.ToLower() == lookupName)
            {
                characterGO = characterSkin.gameObject;
                characterSkin.gameObject.SetActive(true);
                m_activeSkin = characterSkin.gameObject;
                GetComponent<NetworkAnimator>().animator = characterSkin.gameObject.GetComponent<Animator>();
            }
            else
            {
                characterSkin.gameObject.SetActive(false);
            }
        }

        if (characterGO == null)
        {
            Debug.LogWarning($"NetworkPlayer.OnChangeSkin: character {lookupName} not found");
        }

        GetComponent<CharacterData>().m_animator = characterGO.GetComponent<Animator>();
        GetComponent<CharacterData>().m_fishingRodController = characterGO.GetComponent<FishingRodController>();
        GetComponent<CharacterData>().m_characterSkin = characterGO;
        GetComponent<CharacterData>().InitCharacterData(characterSO);
    }

    [Client]
    public void SetFishingRodEnabled(bool _rodeEnabled)
    {
        RequestChangeFishingRodState(_rodeEnabled);
    }

    [Client]
    void OnChangeFishingRodState(bool _oldFishingRodState, bool _newFishingRodState)
    {
        m_activeSkin.GetComponent<FishingRodController>().OnFishingRodStateChange(_newFishingRodState);
    }
    #endregion
}
