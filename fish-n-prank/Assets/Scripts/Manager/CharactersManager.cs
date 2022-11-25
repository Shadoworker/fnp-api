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
    [BoxGroup("Joystick offset")] public float m_xJoystickOffset = 0.2f;
    [BoxGroup("Joystick offset")] public float m_zJoystickOffset = 0.2f;
    private GameObject m_currentCharacter;
    public float m_waterHeight;
    private List<CHARACTER> m_characcterEnumValues;
    private Transform m_playersContainer;

    public void Init()
    {
        m_playersContainer = GameObject.Find(CHARACTER_CONTAINER).transform;
        m_characcterEnumValues = Enum.GetValues(typeof(CHARACTER)).Cast<CHARACTER>().ToList();
        SpawnPlayer();
    }

    public void SetCurrentCharacter(GameObject _character)
    {
        m_currentCharacter = _character;
        m_currentCharacter.SetActive(true);
        GameStateManager.CameraManager.SetTarget(m_currentCharacter);
    }

    public GameObject GetCurrentCharacter()
    {
        return m_currentCharacter;
    }

    public void DestroyCurrentPlayer()
    {
        Destroy(m_currentCharacter.transform.GetChild(0).gameObject);
    }

    public void SpawnPlayer()
    {
        SetCurrentCharacter(Instantiate(m_playerPrefab, m_playersContainer));
        SetPlayerSkin(CHARACTER.RANDOM.ToString());
    }
    public void SetPlayerSkin(string _character)
    {
        CharacterSO characterSO = null;
        CHARACTER characterEnum = m_characcterEnumValues.Where(c => c.ToString().Equals(_character)).FirstOrDefault();
        if (characterEnum == CHARACTER.RANDOM)
            characterSO = m_characters[UnityEngine.Random.Range(0, m_characters.Count)];
        else
            characterSO = m_characters.Where(c => c.m_character == characterEnum).FirstOrDefault();
        GameObject skin = Instantiate(characterSO.m_prefab, m_currentCharacter.transform);
        skin.transform.localPosition = Vector3.zero;
        m_currentCharacter.GetComponent<CharacterData>().m_animator = skin.GetComponent<Animator>();
        m_currentCharacter.GetComponent<CharacterData>().m_fishingRodController = skin.GetComponent<FishingRodController>();
        m_currentCharacter.GetComponent<CharacterData>().InitCharacterData(characterSO);
    }
}
