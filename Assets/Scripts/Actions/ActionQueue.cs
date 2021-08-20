using System.Collections.Generic;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    private Queue<GameAction> m_GameActions = new Queue<GameAction>();
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

        GameAction first = m_GameActions.Peek();
        first.Update(m_GameMap);
        if (first.IsDone)
        {
            GameObject gameObject = first.GameObject;
            bool isSuccess = first.IsSuccess;
            Debug.Log("Complete " + first.GetDebugString() + " with GameObject " + gameObject + " isSuccess " + isSuccess);
            m_GameActions.Dequeue();

            if (gameObject.tag == "Player" && isSuccess)
            {
                List<GameAction> enemyActions = m_GameMap.HandleEnemyTurns();
                foreach (GameAction enemyAction in enemyActions)
                {
                    m_GameActions.Enqueue(enemyAction);
                }
                Tile playerTile = gameObject.GetComponent<Tile>();
                m_GameMap.ComputeFOV(playerTile.X, playerTile.Y);
            }

            if (m_GameActions.Count > 0)
            {
                GameAction gameAction = m_GameActions.Peek();
                gameAction.Apply(m_GameMap);
                if (gameAction.GameObject != m_GameMap.Player)
                {
                    Debug.Log(gameAction.GameObject.name + " : " + gameAction.GetDebugString());
                }
            }
        }
    }

    public void AddAction(GameAction gameAction)
    {
        Debug.Log("Add action " + gameAction.GetDebugString());
        m_GameActions.Enqueue(gameAction);
        if (m_GameActions.Count == 1)
        {
            gameAction.Apply(m_GameMap);
            if (gameAction.GameObject != m_GameMap.Player)
            {
                Debug.Log(gameAction.GetDebugString());
            }
        }
    }

    public bool IsBusy()
    {
        return m_GameActions.Count > 0;
    }
}
