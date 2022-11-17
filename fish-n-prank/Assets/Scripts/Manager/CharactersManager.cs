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
    [BoxGroup("List of characters")] public List<CharacterSO> m_characters;
    [BoxGroup("Game Events")] public GameEvent m_jumpEvent;
    [BoxGroup("Game Events")] public GameEvent m_toggleFishingRodEvent;
    [BoxGroup("Character movement")] public float m_xAxisSensitivity;
    [BoxGroup("Character movement")] public float m_zAxisSensitivity;
    [BoxGroup("Character position")] public Vector3 m_characterSpawnPosition;
    private GameObject m_currentCharacter;
    public float m_waterHeight;
    private List<CHARACTER> m_characcterEnumValues;
    private Transform m_playersContainer;

    public void Init()
    {
        // Character spawning done by Mirror
        //m_playersContainer = GameObject.Find(CHARACTER_CONTAINER).transform;
        //m_characcterEnumValues = Enum.GetValues(typeof(CHARACTER)).Cast<CHARACTER>().ToList();
        //SpawnCharacter(CHARACTER.RANDOM.ToString());
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
        Destroy(m_currentCharacter);
    }

    public void SpawnCharacter(string _character)
    {
        GameObject prefab = null;
        CHARACTER characterEnum = m_characcterEnumValues.Where(c => c.ToString().Equals(_character)).FirstOrDefault();
        if (characterEnum == CHARACTER.RANDOM)
            prefab = m_characters[UnityEngine.Random.Range(0, m_characters.Count)].m_prefab;
        else
            prefab = m_characters.Where(c => c.m_character == characterEnum).FirstOrDefault().m_prefab;
        prefab.transform.localPosition = m_characterSpawnPosition;
        SetCurrentCharacter(Instantiate(prefab, m_playersContainer));
    }
}
