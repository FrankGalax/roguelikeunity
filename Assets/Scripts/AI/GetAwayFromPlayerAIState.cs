using UnityEngine;
using System.Collections.Generic;

public class GetAwayFromPlayerAIState : AIState
{
    public override GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        GameAction effectGameAction = AIHelper.GetEffectGameplayAction(gameObject, player);
        if (effectGameAction != null)
        {
            return effectGameAction;
        }

        Tile playerTile = player.GetComponent<Tile>();
        Tile gameObjectTile = gameObject.GetComponent<Tile>();
        int deltaX = gameObjectTile.X - playerTile.X;
        int deltaY = gameObjectTile.Y - playerTile.Y;
        deltaX = deltaX > 0 ? 1 : (deltaX < 0 ? -1 : 0);
        deltaY = deltaY > 0 ? 1 : (deltaY < 0 ? -1 : 0);

        for (int i = 1; i < 5; ++i)
        {
            int destX = gameObjectTile.X + deltaX * i;
            int destY = gameObjectTile.Y + deltaY * i;

            GameAction gameAction = TryMoveTo(gameObjectTile, gameMap, destX, destY);

            if (gameAction != null)
            {
                return gameAction;
            }
        }

        for (int i = 1; i < 5; ++i)
        {
            int destX = gameObjectTile.X + deltaX * i/2;
            int destY = gameObjectTile.Y + deltaY * i;

            GameAction gameAction = TryMoveTo(gameObjectTile, gameMap, destX, destY);

            if (gameAction != null)
            {
                return gameAction;
            }
        }

        for (int i = 1; i < 5; ++i)
        {
            int destX = gameObjectTile.X + deltaX * i;
            int destY = gameObjectTile.Y + deltaY * i/2;

            GameAction gameAction = TryMoveTo(gameObjectTile, gameMap, destX, destY);

            if (gameAction != null)
            {
                return gameAction;
            }
        }

        return null;
    }

    private GameAction TryMoveTo(Tile gameObjectTile, GameMap gameMap, int x, int y)
    {
        Tile tile = gameMap.GetTileAtLocation(x, y);
        Tile actor = gameMap.GetActorAtLocation(x, y);
        if (tile != null && !tile.IsBlockingMovement(gameObjectTile.gameObject) && actor == null)
        {
            List<(int, int)> path = PathFinding.AStar(gameMap, gameObjectTile.gameObject, gameObjectTile.X, gameObjectTile.Y, x, y);
            if (path != null && path.Count > 1)
            {
                (int, int) direction = (path[1].Item1 - gameObjectTile.X, path[1].Item2 - gameObjectTile.Y);
                return new MoveAction { GameObject = gameObjectTile.gameObject, Direction = direction };
            }
        }

        return null;
    }
}