using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Color Target { get; set; }

    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Target = m_SpriteRenderer.color;
    }

    private void Update()
    {
        m_SpriteRenderer.color = Color.Lerp(m_SpriteRenderer.color, Target, Time.deltaTime * 6.0f);   
    }
}
