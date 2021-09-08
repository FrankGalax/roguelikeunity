using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateRequest
{
    Dungeon,
    SingleTarget,
    AreaTarget
}

public class GameManager : GameSingleton<GameManager>
{
    public int CurrentFloor { get; set; }
    public bool IsPaused { get; set; }

    public float DiedWaitTime = 5.0f;

    private StateMachine m_StateMachine;
    private float m_DiedWaitTimer = -1.0f;

    private void Awake()
    {
        CurrentFloor = 0;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_StateMachine = new StateMachine(new DungeonState());
        SceneManager.sceneLoaded += OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<DamageComponent>().DiedSignal.AddSlot(OnPlayerDied);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_StateMachine != null)
        {
            m_StateMachine.Transition(new DungeonState());
        }
    }

    private void Update()
    {
        if (IsPaused)
        {
            return;
        }

        m_StateMachine.Update();

        if (m_DiedWaitTimer > 0.0f)
        {
            m_DiedWaitTimer -= Time.deltaTime;
            if (m_DiedWaitTimer <= 0.0f)
            {
                SceneManager.LoadScene("mainmenu");
            }
        }
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
            case GameStateRequest.AreaTarget:
                m_StateMachine.Transition(new AreaTargetState());
                break;
        }
    }

    private void OnPlayerDied()
    {
        m_DiedWaitTimer = DiedWaitTime;
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
        m_TargetComponent.TargetTile = null;
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

public class AreaTargetState : State
{
    private TargetInputHandler m_InputHandler;
    private GameObject m_AreaSelection;
    private TargetComponent m_TargetComponent;

    public override void Enter()
    {
        m_InputHandler = new TargetInputHandler();
        m_TargetComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetComponent>();

        m_AreaSelection = GameObject.Instantiate(Config.Instance.AreaSelection);
        int radius = m_TargetComponent.Radius;
        int edge = 2 * radius - 1;

        GameObject topLeft = GameObject.Instantiate(Config.Instance.AreaSelectionTopLeft, m_AreaSelection.transform);
        topLeft.transform.localPosition = new Vector3((float)-radius, (float)radius, 0.0f);
        GameObject bottomLeft = GameObject.Instantiate(Config.Instance.AreaSelectionBottomLeft, m_AreaSelection.transform);
        bottomLeft.transform.localPosition = new Vector3((float)-radius, (float)-radius, 0.0f);
        GameObject bottomRight = GameObject.Instantiate(Config.Instance.AreaSelectionBottomRight, m_AreaSelection.transform);
        bottomRight.transform.localPosition = new Vector3((float)radius, (float)-radius, 0.0f);
        GameObject topRight = GameObject.Instantiate(Config.Instance.AreaSelectionTopRight, m_AreaSelection.transform);
        topRight.transform.localPosition = new Vector3((float)radius, (float)radius, 0.0f);

        for (int i = 0; i < edge; ++i)
        {
            GameObject top = GameObject.Instantiate(Config.Instance.AreaSelectionTop, m_AreaSelection.transform);
            top.transform.localPosition = new Vector3((float)-radius + 1 + i, (float)radius, 0.0f);
            GameObject left = GameObject.Instantiate(Config.Instance.AreaSelectionLeft, m_AreaSelection.transform);
            left.transform.localPosition = new Vector3((float)-radius, (float)-radius + 1 + i, 0.0f);
            GameObject bottom = GameObject.Instantiate(Config.Instance.AreaSelectionBottom, m_AreaSelection.transform);
            bottom.transform.localPosition = new Vector3((float)-radius + 1 + i, (float)-radius, 0.0f);
            GameObject right = GameObject.Instantiate(Config.Instance.AreaSelectionRight, m_AreaSelection.transform);
            right.transform.localPosition = new Vector3((float)radius, (float)-radius + 1 + i, 0.0f);
        }
    }

    public override void Exit()
    {
        GameObject.Destroy(m_AreaSelection);
        m_TargetComponent.TargetTile = null;
        m_TargetComponent.Radius = 0;
    }

    public override void Update()
    {
        m_InputHandler.Update();

        if (m_InputHandler.MouseTile != null)
        {
            m_AreaSelection.transform.position = new Vector3((float)m_InputHandler.MouseTile.X, (float)m_InputHandler.MouseTile.Y, 0.0f);

            if (m_InputHandler.MouseDown)
            {
                m_TargetComponent.TargetTile = m_InputHandler.MouseTile;
            }
        }
    }
}
