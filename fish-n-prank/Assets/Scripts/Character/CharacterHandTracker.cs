using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandTracker : MonoBehaviour
{
    public Transform m_track;
    private Vector3 m_cachedPosition;
    void Start()
    {
        if (m_track)
        {
            m_cachedPosition = m_track.position;
        }
    }

    void Update()
    {
        if (m_track && m_cachedPosition != m_track.position)
        {
            m_cachedPosition = m_track.position;
            transform.position = m_cachedPosition;
        }
    }
}
