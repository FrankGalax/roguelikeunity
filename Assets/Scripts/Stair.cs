using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
    public Sprite FloorSprite;
    public bool IsBlocked { get; private set; }

    private Sprite m_StairsSprite;
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_StairsSprite = m_SpriteRenderer.sprite;
    }

    public void Block()
    {
        m_SpriteRenderer.sprite = FloorSprite;
        IsBlocked = true;
    }

    public void UnBlock()
    {
        if (m_SpriteRenderer != null)
        {
            m_SpriteRenderer.sprite = m_StairsSprite;
            IsBlocked = false;
        }
    }
}
