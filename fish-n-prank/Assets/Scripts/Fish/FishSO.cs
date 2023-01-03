using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Configs/FishSO")]
public class FishSO : ScriptableObject
{


    public string m_fishSpecies;
    [Range(7f, 15f)]
    public float m_battleDuration;
    [Range(4000, 30000f)]
    public float m_speed;
    public GameObject m_prefab;

    public FishSpecies m_species;
    public FishSizeStruct m_sizes = new FishSizeStruct(){
        baby = new FishSize(){size = Size.baby, ratio = 0.5f},
        little = new FishSize(){size = Size.little, ratio = 0.75f},
        normal = new FishSize(){size = Size.normal, ratio = 1.0f},
        big = new FishSize(){size = Size.big, ratio = 1.5f},
        giant = new FishSize(){size = Size.giant, ratio = 2.0f}
    };
    
}

[System.Serializable]
public enum SpeciesName {
    conger,
    discus,
    carp
};

[System.Serializable]
public class Offshore {
    public float min;
    public float max;
};

[System.Serializable]
public enum AvailabilityArea {
    northEast,
    northWest,
    southEast,
    southWest
};

[System.Serializable]
public enum FishingRod {
    rod_1,
    rod_2,
    rod_3
};

[System.Serializable]
public class Mapping {
    public string staging;
    public string prod;
};

[System.Serializable]
public enum Scarcity {
    scarcity_1,
    scarcity_2,
    scarcity_3
};


[System.Serializable]
public class FishSpecies
{
    public SpeciesName name;
    public Offshore offshore;
    public float base_weight;
    public List<AvailabilityArea> availability_area;
    public List<FishingRod> fishing_rod;
    public Mapping id_mapping;
    public Scarcity scarcity;
    public bool isCoralFish;
}

[System.Serializable]
public enum Size {
    baby,
    little,
    normal,
    big,
    giant
};

[System.Serializable]
public class FishSize {
    public Size size = Size.normal;
    public float ratio = 1.0f;
};

[System.Serializable]
public struct FishSizeStruct {
    public FishSize baby;
    public FishSize little;
    public FishSize normal;
    public FishSize big;
    public FishSize giant;
};


[System.Serializable]
public class SelectedFish
{
    public string name;
    public FishSize size;
    public float weight;
    public float m_battleDuration;
    public float m_speed;
}

