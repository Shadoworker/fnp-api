using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public Transform m_hook; // The object to move towards
    public bool m_hasPulledFishingRod;
    public Vector3 m_initialPos;
    private float m_lerpTime = 1f;
    private const float HOOK_POS_RESET_DELAY = 4f;
    private const float SPRING_PULLED = 20F;
    private const float SPRING_DEFAULT = 10f;
    private void Start()
    {
        m_initialPos = transform.localPosition;
    }

    void Update()
    {
        if(m_hasPulledFishingRod)
        {
            Vector3 halfwayPoint = Vector3.Lerp(transform.position, m_hook.position, m_lerpTime);
            transform.position = halfwayPoint;
            m_hasPulledFishingRod = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<SpringJoint>().spring = SPRING_PULLED;
            StartCoroutine(ResetHookPos());
        }
    }

    IEnumerator ResetHookPos()
    {
        yield return new WaitForSeconds(HOOK_POS_RESET_DELAY);
        transform.localPosition = m_initialPos;
        GetComponent<SpringJoint>().spring = SPRING_DEFAULT;
        if (transform.childCount > 1)
            RemoveFishes();
    }

    public void RemoveFishes()
    {
        int index = 0;
        foreach(Transform t in transform)
        {
            if (index != 0)
                Destroy(t.gameObject);
            index++;
        }
    }
}
