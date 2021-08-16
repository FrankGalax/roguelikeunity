using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
    public float Speed = 4.0f;

    private List<(int, int)> m_Path;
    private Tile m_Tile;

    private void Awake()
    {
        m_Tile = GetComponent<Tile>();
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        Tile playerTile = player.GetComponent<Tile>();
        int distance = m_Tile.GetDistance(playerTile);

        if (distance <= 1)
        {
            return new MeleeAction { GameObject = gameObject, Target = player };
        }

        m_Path = PathFinding.AStar(gameMap, m_Tile.X, m_Tile.Y, playerTile.X, playerTile.Y);

        if (m_Path == null || m_Path.Count < 2)
        {
            return null;
        }

        (int, int) direction = (m_Path[1].Item1 - m_Tile.X, m_Path[1].Item2 - m_Tile.Y);
        return new MoveAction { GameObject = gameObject, Direction = direction, Speed = Speed };
    }
}
