using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnChangeSkin))]
    private string m_currentSkinName;

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
    }

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
                //GetComponent<NetworkAnimator>().animator = characterSkin.gameObject.GetComponent<Animator>();
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

        //m_currentSkin.transform.localPosition = Vector3.zero;

        GetComponent<CharacterData>().m_animator = characterGO.GetComponent<Animator>();
        GetComponent<CharacterData>().m_fishingRodController = characterGO.GetComponent<FishingRodController>();
        GetComponent<CharacterData>().InitCharacterData(characterSO);
    }

}
