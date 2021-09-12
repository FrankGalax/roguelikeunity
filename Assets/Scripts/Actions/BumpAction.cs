using UnityEngine;

public class BumpAction : GameAction
{
    public (int, int) Direction { get; set; }

    private GameAction m_SubAction;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Tile gameObjectTile = GameObject.GetComponent<Tile>();
        int destX = gameObjectTile.X + Direction.Item1;
        int destY = gameObjectTile.Y + Direction.Item2;

        if (!GameObject.CompareTag("Player"))
        {
            Tile playerTile = GameObject.FindGameObjectWithTag("Player").GetComponent<Tile>();
            if (playerTile.X == destX && playerTile.Y == destY)
            {
                m_SubAction = new MeleeAction { GameObject = GameObject, Target = playerTile.gameObject };
                m_SubAction.Apply(gameMap);
                IsDone = m_SubAction.IsDone;
                IsSuccess = m_SubAction.IsSuccess;
                return;
            }
        }

        Tile actorTile = gameMap.GetActorAtLocation(destX, destY);
        if (actorTile != null && actorTile.gameObject != GameObject && actorTile.BlocksMovement)
        {
            DamageComponent damageComponent = actorTile.GetComponent<DamageComponent>();
            if (damageComponent != null && !damageComponent.IsInvulnerable)
            {
                m_SubAction = new MeleeAction { GameObject = GameObject, Target = actorTile.gameObject };
                m_SubAction.Apply(gameMap);
                IsDone = m_SubAction.IsDone;
                IsSuccess = m_SubAction.IsSuccess;
                return;
            }
        }

        Tile tile = gameMap.GetTileAtLocation(destX, destY);
        if (tile != null && tile.BlocksMovement)
        {
            if (tile.GetComponent<DoorComponent>() != null && GameObject.GetComponent<OpenDoorComponent>() != null)
            {
                m_SubAction = new OpenDoorAction { GameObject = GameObject, Door = tile.gameObject };
                m_SubAction.Apply(gameMap);
                IsDone = m_SubAction.IsDone;
                IsSuccess = m_SubAction.IsSuccess;
            }
            else
            {
                IsDone = true;
                IsSuccess = false;
            }
        }
        else
        {
            m_SubAction = new MoveAction { GameObject = GameObject, Direction = Direction };
            m_SubAction.Apply(gameMap);
            IsDone = m_SubAction.IsDone;
            IsSuccess = m_SubAction.IsSuccess;
        }
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        if (m_SubAction != null)
        {
            m_SubAction.Update(gameMap);
            IsDone = m_SubAction.IsDone;
        }
    }

    public override void Release(GameMap gameMap)
    {
        base.Release(gameMap);

        if (m_SubAction != null)
        {
            m_SubAction.Release(gameMap);
        }
    }

    public override string GetDebugString()
    {
        if (m_SubAction != null)
        {
            return m_SubAction.GetDebugString();
        }

        return "BumpAction without a sub action";
    }
}