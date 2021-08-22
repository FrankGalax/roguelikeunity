using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    public bool BlocksMovement;
    public bool Transparent;
    public bool AlwaysVisible;

    public int X { get; set; }
    public int Y { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDiscovered { get; set; }

    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        IsVisible = AlwaysVisible;
        IsDiscovered = AlwaysVisible;
        IsDiscovered = false;
        X = (int)transform.position.x;
        Y = (int)transform.position.y;
    }

    public void UpdateVisibility()
    {
        if (AlwaysVisible)
        {
            return;
        }

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

    public int GetDistance(Tile otherTile)
    {
        return Math.Abs(X - otherTile.X) + Math.Abs(Y - otherTile.Y);
    }
}
