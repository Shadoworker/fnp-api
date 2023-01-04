using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FishCollectionCtrlr : MonoBehaviour
{
    public GameObject m_bagFishesContainer;
    public GameObject m_aquariumFishesContainer;

    public GameObject m_fishItemPrefab;

    public List<FishCollectionItem> m_bagFishes;
    public List<FishCollectionItem> m_aquariumFishes;
    // Start is called before the first frame update
    void Start()
    {
        // These data must rely on api callbacks from l3v3l
        m_bagFishes = new List<FishCollectionItem>() {
            new FishCollectionItem(){name = SpeciesName.conger, amount = 10, type = FishType.bag},
            new FishCollectionItem(){name = SpeciesName.discus, amount = 10, type = FishType.bag},
            new FishCollectionItem(){name = SpeciesName.carp, amount = 10,   type = FishType.bag}
        };

        // These data must rely on api callbacks from Backend API
        m_aquariumFishes = new List<FishCollectionItem>() {
            new FishCollectionItem(){name = SpeciesName.conger, amount = 7 ,  type = FishType.aquarium},
            new FishCollectionItem(){name = SpeciesName.discus, amount = 23 , type = FishType.aquarium},
            new FishCollectionItem(){name = SpeciesName.carp, amount = 12 ,   type = FishType.aquarium}
        };


        DisplayFishes();
    }


    public void DisplayFishes()
    {
        // Bag fishes
        foreach (Transform child in m_bagFishesContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < m_bagFishes.Count; i++)
        {
            GameObject item = Instantiate(m_fishItemPrefab, Vector3.zero, Quaternion.identity, m_bagFishesContainer.transform);
            item.transform.GetComponent<FishItemCtrlr>().SetItem(transform, m_bagFishes[i]);
        }

        // Aquarium fishes
        foreach (Transform child in m_aquariumFishesContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < m_aquariumFishes.Count; i++)
        {
            GameObject item = Instantiate(m_fishItemPrefab, Vector3.zero, Quaternion.identity, m_aquariumFishesContainer.transform);
            item.transform.GetComponent<FishItemCtrlr>().SetItem(transform, m_aquariumFishes[i]);
        }
    }


    public void OnFishItemClicked(FishCollectionItem _fci, int _amount)
    {
        if(_fci.amount == 0) return;
        // Debug.Log(_fci.name);
        if(_fci.type == FishType.bag)
        {
           // Decrease
           FishCollectionItem f = m_bagFishes.FirstOrDefault(f=>f.name == _fci.name);
           f.amount = f.amount - _amount;

           // Increase
           FishCollectionItem _f = m_aquariumFishes.FirstOrDefault(f=>f.name == _fci.name);
           _f.amount = _f.amount + _amount;
        }
        else
        {
            // Decrease
           FishCollectionItem f = m_aquariumFishes.FirstOrDefault(f=>f.name == _fci.name);
           f.amount = f.amount - _amount;

           // Increase
           FishCollectionItem _f = m_bagFishes.FirstOrDefault(f=>f.name == _fci.name);
           _f.amount = _f.amount + _amount;
        }


        DisplayFishes();
    }

 
}



[System.Serializable]
public enum FishType {
    bag,
    aquarium
};

[System.Serializable]
public class FishCollectionItem
{
    public SpeciesName name;
    public int amount = 0;
    public FishType type; 

}
