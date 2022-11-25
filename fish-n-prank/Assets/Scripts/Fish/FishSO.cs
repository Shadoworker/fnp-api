using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Configs/FishSO")]
public class FishSO : ScriptableObject
{
    public string m_fishSpecies;
    [Range(7f, 15f)]
    public float m_battleDuration;
    [Range(7000, 30000f)]
    public float m_speed;
}
