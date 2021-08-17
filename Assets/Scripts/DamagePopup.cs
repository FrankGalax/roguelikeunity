using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float FadeTime;
    public Vector3 Offset;

    private float m_FadeTimer;
    private Vector3 m_StartPosition;

    private void Start()
    {
        m_StartPosition = transform.position;
        m_FadeTimer = FadeTime;
    }

    private void Update()
    {
        m_FadeTimer -= Time.deltaTime;
        float ratio = 1.0f - m_FadeTimer / FadeTime;

        transform.position = Vector3.Lerp(m_StartPosition, m_StartPosition + Offset, ratio);
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, ratio);

        if (m_FadeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
