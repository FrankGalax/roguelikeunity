
public class BumpAction : GameAction
{
    public (int, int) Direction { get; set; }
    public float Speed { get; set; }

    private GameAction m_SubAction;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Tile gameObjectTile = GameObject.GetComponent<Tile>();
        int destX = gameObjectTile.X + Direction.Item1;
        int destY = gameObjectTile.Y + Direction.Item2;

        Tile actorTile = gameMap.GetActorAtLocation(destX, destY);
        if (actorTile != null && actorTile.BlocksMovement)
        {
            m_SubAction = new MeleeAction { GameObject = GameObject, Target = actorTile.gameObject };
            m_SubAction.Apply(gameMap);
            IsDone = m_SubAction.IsDone;
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
                m_SubAction = new MoveAction { GameObject = GameObject, Direction = Direction, Speed = Speed };
                m_SubAction.Apply(gameMap);
                IsDone = m_SubAction.IsDone;
            }
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
}