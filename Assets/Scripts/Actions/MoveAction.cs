using UnityEngine;

public class MoveAction : GameAction
{
    public (int, int) Direction { get; set; }

    private Vector3 m_DirectionVector;
    private Vector3 m_Target;
    private float m_Speed;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Tile tile = GameObject.GetComponent<Tile>();

        int destX = tile.X + Direction.Item1;
        int destY = tile.Y + Direction.Item2;

        if (!gameMap.IsInBounds(destX, destY))
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        Tile targetTile = gameMap.GetTileAtLocation(destX, destY);

        if (targetTile != null && targetTile.BlocksMovement)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        Vector3 startPosition = new Vector3((float)tile.X, (float)tile.Y, 0.0f);
        tile.X += Direction.Item1;
        tile.Y += Direction.Item2;
        m_DirectionVector = new Vector3((float)Direction.Item1, (float)Direction.Item2, 0.0f);
        m_Target = startPosition + m_DirectionVector;
        m_Speed = Config.Instance.MoveSpeed;

        AnimationComponent animationComponent = GameObject.GetComponent<AnimationComponent>();
        if (animationComponent != null)
        {
            animationComponent.StartMoving(Direction);
        }
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        if (IsDone)
        {
            return;
        }

        Vector3 distance = m_Target - GameObject.transform.position;

        if (Vector3.Dot(m_DirectionVector, distance) <= 0)
        {
            IsDone = true;
            GameObject.transform.position = m_Target;
        }
        else
        {
            float movement = m_Speed * Time.deltaTime;
            float remainingDistance = distance.magnitude;
            if (movement > remainingDistance)
            {
                IsDone = true;
                GameObject.transform.position = m_Target;
            }
            else
            {
                GameObject.transform.Translate(m_DirectionVector * movement);
            }
        }
    }

    public override void Release(GameMap gameMap)
    {
        base.Release(gameMap);

        AnimationComponent animationComponent = GameObject.GetComponent<AnimationComponent>();
        if (animationComponent != null)
        {
            animationComponent.StopMoving();
        }

        Tile tile = GameObject.GetComponent<Tile>();
        AreaControl areaControl = gameMap.GetAreaControl(tile.X, tile.Y);
        if (areaControl != null)
        {
            areaControl.OnEnter(GameObject);
        }
    }

    public override string GetDebugString()
    {
        return "MoveAction with Direction (" + Direction.Item1 + ", " + Direction.Item2 + ")";
    }
}
