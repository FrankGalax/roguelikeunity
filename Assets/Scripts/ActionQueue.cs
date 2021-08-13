using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    public GameObject GameObject { get; set; }

    public abstract void Apply(GameMap gameMap);

    public virtual void Update(GameMap gameMap) { }

    public abstract bool IsDone();
}

public class MoveAction : GameAction
{
    public (int, int) Direction { get; set; }
    public float Speed { get; set; }

    private Vector3 m_DirectionVector;
    private Vector3 m_Target;
    private bool m_IsDone;

    public override void Apply(GameMap gameMap)
    {
        m_IsDone = false;

        Tile tile = GameObject.GetComponent<Tile>();
        Tile targetTile = gameMap.GetTileAtLocation(tile.X + Direction.Item1, tile.Y + Direction.Item2);

        if (targetTile != null && targetTile.BlocksMovement)
        {
            m_IsDone = true;
            return;
        }

        Vector3 startPosition = new Vector3((float)tile.X, (float)tile.Y, 0.0f);
        tile.X += Direction.Item1;
        tile.Y += Direction.Item2;
        m_DirectionVector = new Vector3((float)Direction.Item1, (float)Direction.Item2, 0.0f);
        m_Target = startPosition + m_DirectionVector;
    }

    public override void Update(GameMap gameMap)
    {
        if (m_IsDone)
        {
            return;
        }

        Vector3 distance = m_Target - GameObject.transform.position;

        if (Vector3.Dot(m_DirectionVector, distance) <= 0)
        {
            m_IsDone = true;
            GameObject.transform.position = m_Target;
        }
        else
        {
            float movement = Speed * Time.deltaTime;
            float remainingDistance = distance.magnitude;
            if (movement > remainingDistance)
            {
                m_IsDone = true;
                GameObject.transform.position = m_Target;
            }
            else
            {
                GameObject.transform.Translate(m_DirectionVector * movement);
            }
        }
    }

    public override bool IsDone()
    {
        return m_IsDone;
    }
}

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
        if (m_GameActions[0].IsDone())
        {
            GameObject gameObject = m_GameActions[0].GameObject;
            if (gameObject.tag == "Player")
            {
                Tile playerTile = gameObject.GetComponent<Tile>();
                m_GameMap.ComputeFOV(playerTile.X, playerTile.Y);
            }
            m_GameActions.RemoveAt(0);
            if (m_GameActions.Count > 0)
            {
                m_GameActions[0].Apply(m_GameMap);
            }
        }
    }

    public void AddAction(GameAction gameAction)
    {
        if (m_GameActions.Count == 0)
        {
            gameAction.Apply(m_GameMap);
        }
        m_GameActions.Add(gameAction);
    }

    public bool IsBusy() { return m_GameActions.Count > 0;}
}
