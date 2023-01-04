using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtrlr : MonoBehaviour
{
    public GameObject m_mainPanel;
    public GameObject m_bagPanel;
    public GameObject m_aquariumPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GotoBag()
    {
        m_mainPanel.SetActive(true);
        m_bagPanel.SetActive(true);
    }

    public void CloseBagPanel()
    {
        m_mainPanel.SetActive(false);
        m_bagPanel.SetActive(false);
        m_aquariumPanel.SetActive(false);
    }
    
    public void GotoAquarium()
    {
        m_bagPanel.SetActive(false);
        m_aquariumPanel.SetActive(true);
    }
    
    
}
