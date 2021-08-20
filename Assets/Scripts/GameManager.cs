using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateRequest
{
    Dungeon,
    SingleTarget
}

public class GameManager : GameSingleton<GameManager>
{
    private StateMachine m_StateMachine;

    private void Awake()
    {
        m_StateMachine = new StateMachine(new DungeonState());
    }

    private void Update()
    {
        m_StateMachine.Update();
    }

    public void RequestGameState(GameStateRequest request)
    {
        switch (request)
        {
            case GameStateRequest.Dungeon:
                m_StateMachine.Transition(new DungeonState());
                break;
            case GameStateRequest.SingleTarget:
                m_StateMachine.Transition(new SingleTargetState());
                break;
        }
    }
}

public class DungeonState : State
{
    private DungeonInputHandler m_InputHandler;

    public override void Enter()
    {
        m_InputHandler = new DungeonInputHandler();
    }

    public override void Update()
    {
        m_InputHandler.Update();
    }
}

public class SingleTargetState : State
{
    private TargetInputHandler m_InputHandler;
    private GameObject m_SingleTarget;
    private TargetComponent m_TargetComponent;

    public override void Enter()
    {
        m_InputHandler = new TargetInputHandler();
        m_SingleTarget = GameObject.Instantiate(Config.Instance.SingleTarget);
        m_TargetComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetComponent>();
    }

    public override void Exit()
    {
        GameObject.Destroy(m_SingleTarget);
    }

    public override void Update()
    {
        m_InputHandler.Update();

        if (m_InputHandler.MouseTile != null)
        {
            m_SingleTarget.transform.position = new Vector3((float)m_InputHandler.MouseTile.X, (float)m_InputHandler.MouseTile.Y, 0.0f);

            if (m_InputHandler.MouseDown)
            {
                m_TargetComponent.TargetTile = m_InputHandler.MouseTile;
            }
        }
    }
}
