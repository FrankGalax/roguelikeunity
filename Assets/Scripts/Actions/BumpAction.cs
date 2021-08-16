
public class BumpAction : GameAction
{
    public (int, int) Direction { get; set; }
    public float Speed { get; set; }

    private MoveAction m_MoveAction;
    private MeleeAction m_MeleeAction;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Tile gameObjectTile = GameObject.GetComponent<Tile>();
        int destX = gameObjectTile.X + Direction.Item1;
        int destY = gameObjectTile.Y + Direction.Item2;

        Tile actorTile = gameMap.GetActorAtLocation(destX, destY);
        if (actorTile != null && actorTile.BlocksMovement)
        {
            m_MeleeAction = new MeleeAction { GameObject = GameObject, Target = actorTile.gameObject };
            m_MeleeAction.Apply(gameMap);
        }
        else
        {
            Tile tile = gameMap.GetTileAtLocation(destX, destY);
            if (tile != null && tile.BlocksMovement)
            {
                IsDone = true;
            }
            else
            {
                m_MoveAction = new MoveAction { GameObject = GameObject, Direction = Direction, Speed = Speed };
                m_MoveAction.Apply(gameMap);
            }
        }
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        if (m_MeleeAction != null)
        {
            m_MeleeAction.Update(gameMap);
            IsDone |= m_MeleeAction.IsDone;
        }

        if (m_MoveAction != null)
        {
            m_MoveAction.Update(gameMap);
            IsDone |= m_MoveAction.IsDone;
        }
    }
}