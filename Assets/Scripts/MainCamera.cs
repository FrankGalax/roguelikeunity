using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private GameObject m_Player;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");    
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Player != null)
        {
            transform.position = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y, transform.position.z);
        }
    }
}
