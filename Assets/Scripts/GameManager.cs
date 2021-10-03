using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Assertions;

public enum GameStateRequest
{
    Dungeon,
    SingleTarget,
    AreaTarget,
    DirectionalTarget
}

public class GameManager : GameSingleton<GameManager>
{
    public int CurrentFloor { get; set; }
    public bool IsPaused { get; set; }
    public List<Reward> FloorRewards { get; private set; }

    public float DiedWaitTime = 5.0f;

    private StateMachine m_StateMachine;
    private float m_DiedWaitTimer = -1.0f;
    private GameObject m_Player;
    private UI m_UI;
    private ActionQueue m_ActionQueue;

    private void Awake()
    {
        CurrentFloor = 0;
        FloorRewards = new List<Reward>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_StateMachine = new StateMachine(new DungeonState());
        SceneManager.sceneLoaded += OnSceneLoaded;

        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Player.GetComponent<DamageComponent>().DiedSignal.AddSlot(OnPlayerDied);
        m_UI = FindObjectOfType<UI>();
        m_ActionQueue = FindObjectOfType<ActionQueue>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_StateMachine != null)
        {
            m_StateMachine.Transition(new DungeonState());
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player != m_Player)
        {
            m_Player = player;
            m_Player.GetComponent<DamageComponent>().DiedSignal.AddSlot(OnPlayerDied);
        }
        m_UI = FindObjectOfType<UI>();
        m_ActionQueue = FindObjectOfType<ActionQueue>();
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
            case GameStateRequest.DirectionalTarget:
                m_StateMachine.Transition(new DirectionalTargetState());
                break;
        }
    }

    public void RequestFloorRewards()
    {
        Assert.IsNotNull(m_Player);

        FloorRewards.Clear();

        SpellComponent spellComponent = m_Player.GetComponent<SpellComponent>();

        foreach (Spell spell in Config.Instance.Spells)
        {
            if (spellComponent.KnowsSpell(spell))
            {
                continue;
            }

            FloorRewards.Add(new LearnSpellReward { Spell = spell });
        }

        if (FloorRewards.Count == 0)
        {
            m_ActionQueue.ChangeFloor();
            return;
        }

        FloorRewards.Shuffle();

        m_UI.ShowRewards();
        m_StateMachine.Transition(new NullState());
    }

    public void SelectReward(int rewardIndex)
    {
        if (rewardIndex < FloorRewards.Count)
        {
            FloorRewards[rewardIndex].Apply(m_Player);
        }
        FloorRewards.Clear();

        m_ActionQueue.ChangeFloor();
        m_StateMachine.Transition(new DungeonState());
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

public class DirectionalTargetState : State
{
    private TargetInputHandler m_InputHandler;
    private GameObject m_DirectionalSelection;
    private GameObject m_DiagonalDirectionalSelection;
    private TargetComponent m_TargetComponent;
    private Tile m_PlayerTile;

    public override void Enter()
    {
        m_InputHandler = new TargetInputHandler();
        m_TargetComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetComponent>();

        m_DirectionalSelection = GameObject.Instantiate(Config.Instance.DirectionalSelection);
        m_DiagonalDirectionalSelection = GameObject.Instantiate(Config.Instance.DiagonalDirectionalSelection);
        m_DirectionalSelection.SetActive(false);
        m_DiagonalDirectionalSelection.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerTile = player.GetComponent<Tile>();
    }

    public override void Exit()
    {
        GameObject.Destroy(m_DirectionalSelection);
        GameObject.Destroy(m_DiagonalDirectionalSelection);
        m_TargetComponent.Direction = null;
    }

    public override void Update()
    {
        m_InputHandler.Update();

        if (m_InputHandler.MouseTile != null)
        {
            (int, int) direction = (m_InputHandler.MouseTile.X - m_PlayerTile.X, m_InputHandler.MouseTile.Y - m_PlayerTile.Y);
            direction.Item1 = direction.Item1 > 0 ? 1 : (direction.Item1 < 0 ? -1 : 0);
            direction.Item2 = direction.Item2 > 0 ? 1 : (direction.Item2 < 0 ? -1 : 0);

            if (direction != (0, 0))
            {
                float x = (float)(m_PlayerTile.X + direction.Item1);
                float y = (float)(m_PlayerTile.Y + direction.Item2);
                m_DirectionalSelection.transform.position = new Vector3(x, y, 0.0f);
                m_DiagonalDirectionalSelection.transform.position = new Vector3(x, y, 0.0f);

                if (direction == (0, 1))
                {
                    m_DirectionalSelection.transform.rotation = Quaternion.identity;
                    m_DirectionalSelection.SetActive(true);
                    m_DiagonalDirectionalSelection.SetActive(false);
                }
                else if (direction == (-1, 1))
                {
                    m_DiagonalDirectionalSelection.transform.rotation = Quaternion.identity;
                    m_DirectionalSelection.SetActive(false);
                    m_DiagonalDirectionalSelection.SetActive(true);
                }
                else if (direction == (-1, 0))
                {
                    m_DirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    m_DirectionalSelection.SetActive(true);
                    m_DiagonalDirectionalSelection.SetActive(false);
                }
                else if (direction == (-1, -1))
                {
                    m_DiagonalDirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    m_DirectionalSelection.SetActive(false);
                    m_DiagonalDirectionalSelection.SetActive(true);
                }
                else if (direction == (0, -1))
                {
                    m_DirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                    m_DirectionalSelection.SetActive(true);
                    m_DiagonalDirectionalSelection.SetActive(false);
                }
                else if (direction == (1, -1))
                {
                    m_DiagonalDirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                    m_DirectionalSelection.SetActive(false);
                    m_DiagonalDirectionalSelection.SetActive(true);
                }
                else if (direction == (1, 0))
                {
                    m_DirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
                    m_DirectionalSelection.SetActive(true);
                    m_DiagonalDirectionalSelection.SetActive(false);
                }
                else if (direction == (1, 1))
                {
                    m_DiagonalDirectionalSelection.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
                    m_DirectionalSelection.SetActive(false);
                    m_DiagonalDirectionalSelection.SetActive(true);
                }

                if (m_InputHandler.MouseDown)
                {
                    m_TargetComponent.Direction = direction;
                }
            }
        }
    }
}

public class NullState : State
{
}
