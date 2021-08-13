using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool BlocksMovement;
    public bool Transparent;

    public int X { get; set; }
    public int Y { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDiscovered { get; set; }

    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        IsVisible = false;
        IsDiscovered = false;
        X = (int)transform.position.x;
        Y = (int)transform.position.y;
    }

    public void UpdateVisibility()
    {
        if (IsVisible)
        {
            m_SpriteRenderer.enabled = true;
            m_SpriteRenderer.color = Color.white;
        }
        else if (IsDiscovered)
        {
            m_SpriteRenderer.enabled = true;
            m_SpriteRenderer.color = new Color(0.2f, 0.2f, 0.2f);
        }
        else
        {
            m_SpriteRenderer.enabled = false;
        }
    }
}
