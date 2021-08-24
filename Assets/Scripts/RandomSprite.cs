using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> Sprites;
    public float ChangeTime = 0.0f;
    public bool Flip = false;

    private float m_ChangeTimer;
    private SpriteRenderer m_SpriteRenderer;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        ChangeSprite();
        m_ChangeTimer = ChangeTime;
    }

    private void Update()
    {
        if (m_ChangeTimer > 0.0f)
        {
            m_ChangeTimer -= Time.deltaTime;
            if (m_ChangeTimer <= 0.0f)
            {
                ChangeSprite();
                m_ChangeTimer = ChangeTime;
            }
        }
    }

    private void ChangeSprite()
    {
        int random = UnityEngine.Random.Range(0, Sprites.Count);
        m_SpriteRenderer.sprite = Sprites[random];
        if (Flip)
        {
            m_SpriteRenderer.flipX = UnityEngine.Random.value > 0.5f;
            m_SpriteRenderer.flipY = UnityEngine.Random.value > 0.5f;
        }
    }
}
