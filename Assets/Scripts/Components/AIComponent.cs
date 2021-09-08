using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIComponent : MonoBehaviour
{
    public int AttackTurns = 1;
    public int PassTurns = 0;
    public bool SetPlayerDistance = false;

    public Func<GameObject, GameObject, GameMap, GameAction> GetActionFunc { get; set; }

    private DamageComponent m_DamageComponent;
    private Animator m_Animator;
    private bool m_IsAttacking;
    private int m_Turns;

    private void Awake()
    {
        m_DamageComponent = GetComponent<DamageComponent>();
        m_Animator = GetComponent<Animator>();
        m_IsAttacking = true;
        m_Turns = 0;
    }

    private void Start()
    {
        if (m_Animator != null)
        {
            UpdateAnimator(GameObject.FindGameObjectWithTag("Player"), true);
        }
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        if (!m_DamageComponent.IsAlive)
        {
            return null;
        }

        GameAction gameAction = null;
        if (GetActionFunc != null)
        {
            gameAction = GetActionFunc(gameObject, player, gameMap);
        }

        if (m_Animator != null)
        {
            UpdateAnimator(player, false);
        }

        return gameAction;
    }

    private void UpdateAnimator(GameObject player, bool isFirstUpdate)
    {
        if (PassTurns > 0)
        {
            if (!isFirstUpdate)
            {
                m_Turns++;
            }

            if (m_IsAttacking)
            {
                if (m_Turns >= AttackTurns)
                {
                    m_IsAttacking = false;
                    m_Turns = 0;
                }
            }
            else
            {
                if (m_Turns >= PassTurns)
                {
                    m_IsAttacking = true;
                    m_Turns = 0;
                }
            }

            m_Animator.SetBool("IsAttacking", m_IsAttacking);
        }

        if (SetPlayerDistance)
        {
            Tile playerTile = player.GetComponent<Tile>();
            Tile tile = GetComponent<Tile>();

            int distance = tile.GetDistance(playerTile);
            m_Animator.SetInteger("PlayerDistance", distance);
        }
    }
}