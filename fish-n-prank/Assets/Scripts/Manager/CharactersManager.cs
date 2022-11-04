using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum CHARACTER
{
    SHIBA,
    GRUMPY_CAT,
    KHABY_LAME,
    SALT_BAE,
    TROLL_MAN
}

[CreateAssetMenu(menuName = "Managers/Characters Manager")]
public class CharactersManager : ScriptableObject
{

    [FancyHeader("  Characters Manager  ", 1.5f, "orange", 5.5f, order = 0)]
    [BoxGroup("List of characters")] public List<CharacterSO> m_characters;
    [BoxGroup("Game Events")] public GameEvent m_jumpEvent;
    [BoxGroup("Game Events")] public GameEvent m_toggleFishingRodEvent;

    [BoxGroup("Character movement")] public float m_xAxisSensitivity;
    [BoxGroup("Character movement")] public float m_zAxisSensitivity;
    private GameObject m_currentCharacter;
    public float m_waterHeight;


    public void SetCurrentCharacter(GameObject _character)
    {
        m_currentCharacter = _character;
    }

    public GameObject GetCurrentCharacter()
    {
        return m_currentCharacter;
    }
}
