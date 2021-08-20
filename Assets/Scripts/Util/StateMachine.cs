using UnityEngine;

public abstract class State
{
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual GameAction GetAction(GameMap gameMap, GameObject player) { return null; }
}

public class StateMachine
{
    private State m_State;

    public StateMachine(State state)
    {
        m_State = state;
        if (m_State != null)
        {
            m_State.Enter();
        }
    }

    public void Transition(State state)
    {
        if (m_State != null)
        {
            m_State.Exit();
        }

        m_State = state;

        if (m_State != null)
        {
            m_State.Enter();
        }
    }

    public void Update()
    {
        if (m_State != null)
        {
            m_State.Update();
        }
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        if (m_State != null)
        {
            return m_State.GetAction(gameMap, player);
        }

        return null;
    }
}
