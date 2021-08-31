using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIComponent : MonoBehaviour
{
    public AIBehavior AIBehavior;

    private DamageComponent m_DamageComponent;
    private int m_Index;
    private int m_NbTurns;

    private void Awake()
    {
        m_DamageComponent = GetComponent<DamageComponent>();
        m_Index = 0;
        m_NbTurns = 0;
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        if (!m_DamageComponent.IsAlive)
        {
            return null;
        }

        AIStateTurn aiStateTurn = AIBehavior.AIStateTurns[m_Index];
        AIState aiState = aiStateTurn.AIState;

        GameAction gameAction = aiState.GetAction(gameObject, player, gameMap);
        m_NbTurns++;
        if (m_NbTurns >= aiStateTurn.NbTurns)
        {
            m_Index++;
            if (m_Index >= AIBehavior.AIStateTurns.Count)
            {
                m_Index = 0;
            }
            m_NbTurns = 0;
        }

        return gameAction;
    }
}