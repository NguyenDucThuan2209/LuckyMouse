using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform m_target;
    [SerializeField] bool m_followOnX;
    [SerializeField] bool m_followOnY;
    [SerializeField] bool m_followOnZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_followOnX)
        {
            transform.position = new Vector3(m_target.position.x, transform.position.y, transform.position.z);
        }
        if (m_followOnY)
        {
            transform.position = new Vector3(transform.position.x, m_target.position.y, transform.position.z);
        }
        if (m_followOnZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, m_target.position.z);
        }
    }
}
