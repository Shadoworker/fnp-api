using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu(menuName = "Managers/Fishes Manager")]
public class FishesManager : ScriptableObject
{

    [FancyHeader("Fishes Manager  ", 1.5f, "blue", 5.5f, order = 0)]
    [BoxGroup("List of fishes")] public List<FishSO> m_fishes;

    public FishSO GetRandomFish()
    {
        return m_fishes[Random.Range(0, m_fishes.Count)];
    }

}
