using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.ObjectModel;
using System;

public enum CHARACTER
{
    RANDOM,
    SHIBA,
    GRUMPY_CAT,
    KHABY_LAME,
    SALT_BAE,
    TROLL_MAN,
    CUBE
}

[CreateAssetMenu(menuName = "Managers/Characters Manager")]
public class CharactersManager : ScriptableObject
{
    private const string CHARACTER_CONTAINER = "Players";
    [FancyHeader("  Characters Manager  ", 1.5f, "orange", 5.5f, order = 0)]
    public GameObject m_playerPrefab;
    [BoxGroup("List of characters")] public List<CharacterSO> m_characters;
    [BoxGroup("Game Events")] public GameEvent m_jumpEvent;
    [BoxGroup("Game Events")] public GameEvent m_toggleFishingRodEvent;
    [BoxGroup("Character movement")] public float m_xAxisSensitivity;
    [BoxGroup("Character movement")] public float m_zAxisSensitivity;
    [BoxGroup("Character position")] public Vector3 m_characterSpawnPosition;
    private GameObject m_localPlayer;
    public GameObject LocalPlayer { get { return m_localPlayer; } }
    private GameObject m_currentSkin;
    public float m_waterHeight;
    private List<CHARACTER> m_characcterEnumValues;
    private Transform m_playersContainer;

    public void Init()
    {
    }

    public void SetLocalCharacter(GameObject _character)
    {
        m_currentSkin = _character;
        m_currentSkin.SetActive(true);
        GameStateManager.CameraManager.SetTarget(m_currentSkin);
    }

    public GameObject GetCurrentCharacter()
    {
        return m_currentSkin;
    }

    public void DestroyCurrentPlayer()
    {
        Destroy(m_currentSkin.transform.GetChild(0).gameObject);
    }

    public void SetPlayerSkin(GameObject _localPlayer, string _character)
    {
        CharacterSO characterSO = null;
        CHARACTER characterEnum = m_characcterEnumValues.Where(c => c.ToString().Equals(_character)).FirstOrDefault();
        if (characterEnum == CHARACTER.RANDOM)
            characterSO = m_characters[UnityEngine.Random.Range(0, m_characters.Count)];
        else
            characterSO = m_characters.Where(c => c.m_character == characterEnum).FirstOrDefault();

        m_localPlayer = _localPlayer;
        GameObject characterSkin = Instantiate(characterSO.m_prefab, _localPlayer.transform) ;
        SetLocalCharacter(characterSkin);

        characterSkin.transform.localPosition = Vector3.zero;

        m_localPlayer.GetComponent<CharacterData>().m_animator = characterSkin.GetComponent<Animator>();
        m_localPlayer.GetComponent<CharacterData>().m_fishingRodController = characterSkin.GetComponent<FishingRodController>();
        m_localPlayer.GetComponent<CharacterData>().InitCharacterData(characterSO);
    }
}
