using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    public bool BlocksMovement;
    public bool Transparent;
    public bool AlwaysVisible;
    public int Radius = 0;

    public int X { get; set; }
    public int Y { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDiscovered { get; set; }

    private SpriteRenderer m_SpriteRenderer;
    private ColorComponent m_ColorComponent;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_ColorComponent = GetComponent<ColorComponent>();

        IsVisible = AlwaysVisible;
        IsDiscovered = AlwaysVisible;
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
            UpdateVisibility(transform, true, Color.white);
        }
        else if (IsDiscovered)
        {
            UpdateVisibility(transform, true, new Color(0.2f, 0.2f, 0.2f));
        }
        else
        {
            UpdateVisibility(transform, false, Color.white);
        }
    }

    public int GetDistance(Tile otherTile)
    {
        return Math.Abs(X - otherTile.X) + Math.Abs(Y - otherTile.Y);
    }

    public int GetRadius(int x, int y)
    {
        return Math.Max(Math.Abs(X - x), Math.Abs(Y - y));
    }

    private void UpdateVisibility(Transform transform, bool visible, Color color)
    {
        SpriteRenderer spriteRenderer = transform.gameObject == gameObject ? m_SpriteRenderer : transform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
            if (visible)
            {
                ColorComponent colorComponent = transform.gameObject == gameObject ? m_ColorComponent : transform.GetComponent<ColorComponent>();
                if (colorComponent != null)
                {
                    colorComponent.SetBaseColor(color == Color.white ? colorComponent.InitialColor : color);
                }
                else
                {
                    spriteRenderer.color = color;
                }
            }
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            UpdateVisibility(child, visible, color);
        }
    }
}
