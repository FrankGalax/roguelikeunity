using UnityEngine;

public class OpenDoorAction : GameAction
{
    public GameObject Door;
    public float WaitTime = 0.1f;

    private float m_Timer;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        if (Door == null || Door.GetComponent<DoorComponent>() == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        Tile tile = Door.GetComponent<Tile>();
        int x = tile.X;
        int y = tile.Y;
        gameMap.RemoveTile(x, y);
        gameMap.AddTile(gameMap.Floor, x, y);
        m_Timer = WaitTime;
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
        {
            IsDone = true;
        }
    }

    public override string GetDebugString()
    {
        return "OpenDoorAction with Target " + (GameObject != null ? GameObject.name : "NULL");
    }
}