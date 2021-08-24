using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationComponent : MonoBehaviour
{
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    private float m_StopMovingTimer;
    private int m_LastMoveX;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_LastMoveX = -1;
    }

    private void Update()
    {
        if (m_StopMovingTimer > 0.0f)
        {
            m_StopMovingTimer -= Time.deltaTime;
            if (m_StopMovingTimer <= 0.0f)
            {
                m_Animator.SetFloat("Speed", 0.0f);
            }
        }
    }

    public void StartMoving((int, int) direction)
    {
        if (direction.Item1 != m_LastMoveX && direction.Item1 != 0)
        {
            m_SpriteRenderer.flipX = direction.Item1 > 0;
            m_LastMoveX = direction.Item1;
        }
        m_StopMovingTimer = 0.0f;
        m_Animator.SetFloat("Speed", 1.0f);    
    }

    public void StopMoving()
    {
        m_StopMovingTimer = 0.1f;
    }
}
