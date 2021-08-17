using System.Collections.Generic;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    private List<GameAction> m_GameActions = new List<GameAction>();
    private GameMap m_GameMap;

    private void Awake()
    {
        m_GameMap = GetComponent<GameMap>();
    }

    private void Update()
    {
        if (m_GameActions.Count == 0)
        {
            return;
        }

        m_GameActions[0].Update(m_GameMap);
        if (m_GameActions[0].IsDone)
        {
            GameObject gameObject = m_GameActions[0].GameObject;
            m_GameActions.RemoveAt(0);

            if (gameObject.tag == "Player")
            {
                m_GameActions.AddRange(m_GameMap.HandleEnemyTurns());
                Tile playerTile = gameObject.GetComponent<Tile>();
                m_GameMap.ComputeFOV(playerTile.X, playerTile.Y);
            }

            if (m_GameActions.Count > 0)
            {
                GameAction gameAction = m_GameActions[0];
                gameAction.Apply(m_GameMap);
                if (gameAction.GameObject != m_GameMap.Player)
                {
                    Debug.Log(gameAction.GetDebugString());
                }
            }
        }
    }

    public void AddAction(GameAction gameAction)
    {
        if (m_GameActions.Count == 0)
        {
            gameAction.Apply(m_GameMap);
            if (gameAction.GameObject != m_GameMap.Player)
            {
                Debug.Log(gameAction.GetDebugString());
            }
        }
        m_GameActions.Add(gameAction);
    }

    public bool IsBusy()
    {
        return m_GameActions.Count > 0;
    }
}
