using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIComponent : MonoBehaviour
{
    private DamageComponent m_DamageComponent;
    private StateMachine m_StateMachine;

    private void Awake()
    {
        m_DamageComponent = GetComponent<DamageComponent>();
        m_StateMachine = new StateMachine(new GoToPlayerState(gameObject));
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        if (!m_DamageComponent.IsAlive)
        {
            return null;
        }

        return m_StateMachine.GetAction(gameMap, player);
    }

    public void Confuse(int nbTurns)
    {
        m_StateMachine.Transition(new ConfuseState(gameObject, nbTurns, () => m_StateMachine.Transition(new GoToPlayerState(gameObject))));
    }
}

public class GoToPlayerState : State
{
    private GameObject m_GameObject;

    public GoToPlayerState(GameObject gameObject)
    {
        m_GameObject = gameObject;
    }

    public override GameAction GetAction(GameMap gameMap, GameObject player)
    {
        return new GoToPlayerAction { GameObject = m_GameObject, Player = player };
    }
}

public class ConfuseState : State
{
    private GameObject m_GameObject;
    private int m_NbTurns;
    private Action m_Callback;

    public ConfuseState(GameObject gameObject, int nbTurns, Action callback)
    {
        m_GameObject = gameObject;
        m_NbTurns = nbTurns;
        m_Callback = callback;
    }

    public override GameAction GetAction(GameMap gameMap, GameObject player)
    {
        m_NbTurns--;
        if (m_NbTurns < 0)
        {
            m_Callback();
        }

        int r = UnityEngine.Random.Range(0, 4);
        (int, int) direction = (0, 0);
        switch (r)
        {
            case 0:
                direction = (0, 1);
                break;
            case 1:
                direction = (-1, 0);
                break;
            case 2:
                direction = (0, -1);
                break;
            case 3:
                direction = (1, 0);
                break;
        }

        return new BumpAction() { GameObject = m_GameObject, Direction = direction };
    }
}
