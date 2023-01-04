using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishItemCtrlr : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public float PRESS_DURATION = 1.0f;

    public FishCollectionCtrlr m_fishColCtrlr;

    public Image m_avatar;
    public TextMeshProUGUI m_amountText;

    FishCollectionItem m_fci;

    bool m_pressed = false;
    float m_elapsedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Set
    public void SetItem(Transform _parent, FishCollectionItem _fci)
    {
        m_fishColCtrlr = _parent.GetComponent<FishCollectionCtrlr>();
        m_fci = _fci;
        m_amountText.text = m_fci.amount.ToString();

        m_avatar.GetComponent<Image>().color = m_fci.amount == 0 ? new Color32(53, 53, 53, 255) : new Color32(255, 255, 255, 255);
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // m_fishColCtrlr.OnFishItemClicked(m_fci);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        m_pressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        
        if(m_elapsedTime < PRESS_DURATION)
        {
            m_fishColCtrlr.OnFishItemClicked(m_fci, 1);
        }
        else
        {
            m_fishColCtrlr.OnFishItemClicked(m_fci, m_fci.amount);
        }

        m_pressed = false;
        m_elapsedTime = 0.0f;
    
    }

    void Update()
    {
        if(m_pressed)
            m_elapsedTime += Time.deltaTime;
    }
}
