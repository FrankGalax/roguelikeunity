using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public float Speed;
    public float Amplitude;

    private float m_Timer;

    private void Update()
    {
        m_Timer += Time.deltaTime * Speed;
        
        if (m_Timer > 2 * Mathf.PI)
        {
            m_Timer -= 2 * Mathf.PI;
        }

        transform.localPosition = new Vector3(0.0f, Amplitude * Mathf.Sin(m_Timer), 0.0f);
    }
}
