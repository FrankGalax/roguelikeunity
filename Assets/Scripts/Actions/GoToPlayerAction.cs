using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayerAction : GameAction
{
    public GameObject Player { get; set; }

    private GameAction m_SubAction;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        if (Player == null || GameObject == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        Tile tile = GameObject.GetComponent<Tile>();
        Tile playerTile = Player.GetComponent<Tile>();
        int distance = tile.GetDistance(playerTile);

        if (distance <= 1)
        {
            m_SubAction = new MeleeAction { GameObject = GameObject, Target = Player };
            m_SubAction.Apply(gameMap);
            IsDone = m_SubAction.IsDone;
            IsSuccess = m_SubAction.IsSuccess;
            return;
        }

        List<(int, int)> path = PathFinding.AStar(gameMap, tile.X, tile.Y, playerTile.X, playerTile.Y);

        if (path == null || path.Count < 2)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        (int, int) direction = (path[1].Item1 - tile.X, path[1].Item2 - tile.Y);
        m_SubAction = new MoveAction { GameObject = GameObject, Direction = direction };
        m_SubAction.Apply(gameMap);
        IsDone = m_SubAction.IsDone;
        IsSuccess = m_SubAction.IsSuccess;
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        if (m_SubAction != null)
        {
            m_SubAction.Update(gameMap);
            IsDone = m_SubAction.IsDone;
            IsSuccess = m_SubAction.IsSuccess;
        }
    }

    public override string GetDebugString()
    {
        if (m_SubAction != null)
        {
            return m_SubAction.GetDebugString();
        }

        return "GoToPlayerAction without a sub action";
    }
}
