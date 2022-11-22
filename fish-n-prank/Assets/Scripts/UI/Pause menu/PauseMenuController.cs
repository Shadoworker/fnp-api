using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject m_trollmanObj, m_shibaObj, m_saltBae, m_khabyLame, m_cube, m_grumpyCat;

    public void SpawnCharacter(string _character)
    {
        GameStateManager.CharactersManager.DestroyCurrentPlayer();
        GameStateManager.CharactersManager.SetPlayerSkin(GameStateManager.CharactersManager.LocalPlayer, _character);
    }
}
