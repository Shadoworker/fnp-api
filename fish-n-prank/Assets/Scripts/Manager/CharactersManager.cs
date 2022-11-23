using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using Mirror;

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
    public GameObject LocalPlayer { get;  set; }
    private GameObject m_currentSkin;
    public float m_waterHeight;
    private List<CHARACTER> m_characterEnumValues;
    private Transform m_playersContainer;

    public void Init()
    {
        m_characterEnumValues = Enum.GetValues(typeof(CHARACTER)).Cast<CHARACTER>().ToList();
    }

    public GameObject GetCurrentSkin()
    {
        return m_currentSkin;
    }

    public void DestroyCurrentPlayer()
    {
        Destroy(m_currentSkin.transform.GetChild(0).gameObject);
    }

    // TODO: limit to server
    public GameObject SetPlayerSkin(GameObject _player, string _character)
    {
        CharacterSO characterSO = null;
        CHARACTER characterEnum = m_characterEnumValues.Where(c => c.ToString().Equals(_character)).FirstOrDefault();

        if (characterEnum == CHARACTER.RANDOM)
            characterSO = m_characters[UnityEngine.Random.Range(0, m_characters.Count)];
        else
            characterSO = m_characters.Where(c => c.m_character == characterEnum).FirstOrDefault();

        //GameObject characterSkin = Instantiate(characterSO.m_prefab, _player.transform) ;
        m_currentSkin = _player.transform.GetChild(0).gameObject;
        //m_currentSkin.SetActive(true);

        m_currentSkin.transform.localPosition = Vector3.zero;

        // should do CharacterController.InitCapsuleCollider (and more?)

        _player.GetComponent<CharacterData>().m_animator = m_currentSkin.GetComponent<Animator>();
        _player.GetComponent<CharacterData>().m_fishingRodController = m_currentSkin.GetComponent<FishingRodController>();
        return m_currentSkin;
    }

    // TODO: limit to local client
    public void InitLocalPlayer(GameObject _player)
    {
        LocalPlayer = _player;

        
        //CHARACTER characterEnum = m_characterEnumValues.Where(c => c.ToString().Equals("GRUMPY_CAT")).FirstOrDefault();
        //CharacterSO characterSO = m_characters.Where(c => c.m_character == characterEnum).FirstOrDefault();

        //SetPlayerSkin(_player, "GRUMPY_CAT"); // hack

        //LocalPlayer.GetComponent<CharacterData>().InitCharacterController();
    }
}
