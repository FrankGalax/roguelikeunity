using System.Collections.Generic;
using UnityEngine;

public class AllAtOnceActionQueue : ActionQueue
{
    private List<GameAction> m_GameActions;
    private GameMap m_GameMap;
    private GameObject m_Player;

    private void Awake()
    {
        m_GameActions = new List<GameAction>();
        m_GameMap = GetComponent<GameMap>();
    }

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (m_GameActions.Count == 0)
        {
            return;
        }

        for (int i = m_GameActions.Count - 1; i >= 0; --i)
        {
            GameAction gameAction = m_GameActions[i];
            gameAction.Update(m_GameMap);
            if (gameAction.IsDone)
            {
                gameAction.Release(m_GameMap);
                m_GameActions.RemoveAt(i);

                if (gameAction.IsSuccess && gameAction.GameObject != null && gameAction.GameObject.CompareTag("Player"))
                {
                    Tile playerTile = gameAction.GameObject.GetComponent<Tile>();
                    Tile stairsTile = m_GameMap.GetStairs();
                    if (stairsTile != null && playerTile.X == stairsTile.X && playerTile.Y == stairsTile.Y)
                    {
                        Stair stair = stairsTile.GetComponent<Stair>();
                        if (!stair.IsBlocked)
                        {
                            ChangeFloor();
                            return;
                        }
                    }
                    
                    if (gameAction.IsPausing)
                    {
                        AddEnemyActions();
                        return;
                    }
                }

                if (m_GameActions.Count == 0 && m_Player != null)
                {
                    Tile playerTile = m_Player.GetComponent<Tile>();
                    m_GameMap.ComputeFOV(playerTile.X, playerTile.Y);
                    m_GameMap.EndTurn();
                }
            }
        }
    }

    public override void AddAction(GameAction gameAction)
    {
        m_GameActions.Add(gameAction);
        gameAction.Apply(m_GameMap);

        if (gameAction.IsSuccess && !gameAction.IsPausing && gameAction.GameObject.CompareTag("Player"))
        {
            AddEnemyActions();
        }
    }

    private void AddEnemyActions()
    {
        List<GameAction> enemyActions = m_GameMap.HandleEnemyTurns();
        foreach (GameAction enemyAction in enemyActions)
        {
            m_GameActions.Add(enemyAction);
            enemyAction.Apply(m_GameMap);
        }
    }

    public override bool IsBusy()
    {
        return m_GameActions.Count > 0;
    }
}