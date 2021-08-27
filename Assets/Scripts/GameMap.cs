using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RectangularRoom
{
    public RectangularRoom(int x, int y, int width, int height)
    {
        X1 = x;
        Y1 = y;
        X2 = x + width;
        Y2 = y + height;
    }

    public bool Intersects(RectangularRoom otherRoom)
    {
        return X1 <= otherRoom.X2 &&
            X2 >= otherRoom.X1 &&
            Y1 <= otherRoom.Y2 &&
            Y2 >= otherRoom.Y1;
    }

    public (int, int) GetCenter()
    {
        return ((X1 + X2) / 2, (Y1 + Y2) / 2);
    }

    public int X1 { get; private set; }
    public int Y1 { get; private set; }
    public int X2 { get; private set; }
    public int Y2 { get; private set; }
}

public enum ProcGenAlgo
{
    SquareRoomsWithTunnels,
    Brogue
}

public class GameMap : MonoBehaviour
{
    public GameObject Floor;
    public GameObject Wall;
    public GameObject Rat;
    public GameObject Troll;
    public GameObject HealthPotion;
    public GameObject LightningMedalion;
    public GameObject ConfusionMedalion;
    public GameObject FireMedalion;
    public GameObject Stairs;
    public GameObject Door;
    public int MaxRooms;
    public int RoomMinSize;
    public int RoomMaxSize;
    public int DungeonWidth;
    public int DungeonHeight;
    public ProcGenAlgo ProcGenAlgo = ProcGenAlgo.SquareRoomsWithTunnels;

    private List<List<Tile>> m_Tiles;
    private HashSet<(int, int)> m_CantDestroy;
    private List<Tile> m_Actors;
    private List<AreaControl> m_AreaControls;
    private Tile m_Stairs;
    private GameObject m_Player;
    private FloorDefinition m_FloorDefinition;

    private void Awake()
    {
        m_Tiles = new List<List<Tile>>();
        m_Actors = new List<Tile>();
        m_AreaControls = new List<AreaControl>();
        for (int i = 0; i < DungeonWidth; ++i)
        {
            List<Tile> column = new List<Tile>();
            for (int j = 0; j < DungeonHeight; ++j)
            {
                column.Add(null);
            }
            m_Tiles.Add(column);
        }
        m_CantDestroy = new HashSet<(int, int)>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        int playerX = 0;
        int playerY = 0;

        if (ProcGenAlgo == ProcGenAlgo.SquareRoomsWithTunnels)
        {
            int currentFloor = GameManager.Instance.CurrentFloor;
            if (currentFloor >= Config.Instance.FloorDefinitions.Count)
            {
                m_FloorDefinition = Config.Instance.FloorDefinitions[Config.Instance.FloorDefinitions.Count - 1];
            }
            else
            {
                m_FloorDefinition = Config.Instance.FloorDefinitions[currentFloor];
            }

            List<RectangularRoom> rooms = new List<RectangularRoom>();
            List<List<(int, int)>> tunnels = new List<List<(int, int)>>();

            for (int i = 0; i < MaxRooms; ++i)
            {
                int roomWidth = UnityEngine.Random.Range(RoomMinSize, RoomMaxSize + 1);
                int roomHeight = UnityEngine.Random.Range(RoomMinSize, RoomMaxSize + 1);

                int x = UnityEngine.Random.Range(0, DungeonWidth - roomWidth);
                int y = UnityEngine.Random.Range(0, DungeonHeight - roomHeight);

                RectangularRoom newRoom = new RectangularRoom(x, y, roomWidth, roomHeight);

                bool intersect = false;
                foreach (RectangularRoom otherRoom in rooms)
                {
                    if (otherRoom.Intersects(newRoom))
                    {
                        intersect = true;
                        break;
                    }
                }

                if (intersect)
                {
                    continue;
                }

                SpawnEntities(newRoom);

                if (rooms.Count == 0)
                {
                    (int, int) roomCenter = newRoom.GetCenter();
                    m_Player.transform.position = new Vector3((float)roomCenter.Item1, (float)roomCenter.Item2, 0.0f);
                    Tile playerTile = m_Player.GetComponent<Tile>();
                    playerTile.X = roomCenter.Item1;
                    playerTile.Y = roomCenter.Item2;
                    Camera.main.transform.position = new Vector3(
                        m_Player.transform.position.x,
                        m_Player.transform.position.y,
                        Camera.main.transform.position.z
                    );
                    playerX = roomCenter.Item1;
                    playerY = roomCenter.Item2;

                    Tile actor = GetActorAtLocation(roomCenter.Item1, roomCenter.Item2);
                    if (actor != null)
                    {
                        RemoveActor(actor);
                    }
                }
                else
                {
                    (int, int) newRoomCenter = newRoom.GetCenter();
                    (int, int) lastRoomCenter = rooms[rooms.Count - 1].GetCenter();
                    List<(int, int)> tunnel = TunnelBetween(newRoomCenter.Item1, newRoomCenter.Item2, lastRoomCenter.Item1, lastRoomCenter.Item2);
                    SpawnTunnel(tunnel);
                    tunnels.Add(tunnel);
                }

                rooms.Add(newRoom);
            }

            foreach (List<(int, int)> tunnel in tunnels)
            {
                SpawnTunnelWalls(tunnel);
            }

            SpawnStairs(rooms[rooms.Count - 1]);
        }
        else if (ProcGenAlgo == ProcGenAlgo.Brogue)
        {
            List<List<char>> dungeon = BrogueGen.GenerateDungeon(DungeonWidth, DungeonHeight);

            for (int i = 0; i < DungeonWidth; ++i)
            {
                for (int j = 0; j < DungeonHeight; ++j)
                {
                    switch (dungeon[i][j])
                    {
                        case '#':
                            AddTile(Wall, i, j, false, false);
                            break;
                        case '.':
                            AddTile(Floor, i, j, false, false);
                            break;
                        case 'd':
                            AddTile(Door, i, j, false, false);
                            break;
                    }
                }
            }
        }

        ComputeFOV(playerX, playerY);
    }

    public Tile GetTileAtLocation(int x, int y)
    {
        if (!IsInBounds(x, y))
        {
            return null;
        }
        return m_Tiles[x][y];
    }

    public Tile GetActorAtLocation(int x, int y)
    {
        foreach (Tile actor in m_Actors)
        {
            if (actor.X == x && actor.Y == y)
            {
                return actor;
            }
        }

        return null;
    }

    public List<Tile> GetVisibleEnemies()
    {
        List<Tile> visibleActors = new List<Tile>();
        foreach (Tile actor in m_Actors)
        {
            if (actor.IsVisible && actor.GetComponent<DamageComponent>() != null)
            {
                visibleActors.Add(actor);
            }
        }

        return visibleActors;
    }

    public List<Tile> GetEnemiesInRange(int x, int y, int radius)
    {
        List<Tile> enemies = new List<Tile>();
        foreach (Tile actor in m_Actors)
        {
            if (actor.GetComponent<DamageComponent>() != null)
            {
                int distance = actor.GetRadius(x, y);
                if (distance <= radius)
                {
                    enemies.Add(actor);
                }
            }
        }

        return enemies;
    }

    public List<Tile> GetFloorsInRange(int x, int y, int radius)
    {
        List<Tile> floors = new List<Tile>();
        foreach (List<Tile> column in m_Tiles)
        {
            foreach (Tile tile in column)
            {
                if (tile != null && tile.Transparent && !tile.BlocksMovement)
                {
                    int distance = tile.GetRadius(x, y);
                    if (distance <= radius)
                    {
                        floors.Add(tile);
                    }
                }
            }
        }

        return floors;
    }

    public Tile GetStairs()
    {
        return m_Stairs;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < DungeonWidth && y >= 0 && y < DungeonHeight;
    }

    public void ComputeFOV(int x, int y)
    {
        ForEachTile(tile => { tile.IsVisible = false; });

        FOV.ComputeFOVCircularRaycasting(this, x, y, 8, true, (x, y, isVisible) =>
        {
            Tile tile = m_Tiles[x][y];
            if (tile != null)
            {
                tile.IsVisible = true;
            }
        });

        ForEachTile(tile =>
        {
            tile.IsDiscovered = tile.IsDiscovered || tile.IsVisible;
            tile.UpdateVisibility();
        });

        ForEachActor(actorTile =>
        {
            Tile tile = GetTileAtLocation(actorTile.X, actorTile.Y);
            actorTile.IsVisible = tile != null && tile.IsVisible;
            actorTile.UpdateVisibility();
        });
    }

    public List<GameAction> HandleEnemyTurns()
    {
        Debug.Log("Handle Enemy Turns");
        List<GameAction> gameActions = new List<GameAction>();

        foreach (Tile tile in m_Actors)
        {
            if (!tile.IsVisible)
            {
                continue;
            }

            AIComponent aiComponent = tile.GetComponent<AIComponent>();
            if (aiComponent == null)
            {
                continue;
            }

            GameAction action = aiComponent.GetAction(this, m_Player);
            if (action != null)
            {
                gameActions.Add(action);
            }
        }

        return gameActions;
    }

    public void RemoveActor(Tile tile)
    {
        m_Actors.Remove(tile);
        Destroy(tile.gameObject);
    }

    public void AddAreaControl(AreaControl areaControl)
    {
        m_AreaControls.Add(areaControl);
        Tile areaControlTile = areaControl.GetComponent<Tile>();
        foreach (Tile actor in m_Actors)
        {
            if (actor.X == areaControlTile.X && actor.Y == areaControlTile.Y)
            {
                areaControl.OnEnter(actor.gameObject);
            }
        }

        Tile playerTile = m_Player.GetComponent<Tile>();
        if (playerTile.X == areaControlTile.X && playerTile.Y == areaControlTile.Y)
        {
            areaControl.OnEnter(m_Player);
        }
    }

    public void RemoveAreaControl(AreaControl areaControl)
    {
        m_AreaControls.Remove(areaControl);
    }

    public AreaControl GetAreaControl(int x, int y)
    {
        foreach (AreaControl areaControl in m_AreaControls)
        {
            Tile tile = areaControl.GetComponent<Tile>();
            if (tile != null && tile.X == x && tile.Y == y)
            {
                return areaControl;
            }
        }

        return null;
    }

    public void EndTurn()
    {
        for (int i = m_AreaControls.Count - 1; i >= 0; --i)
        {
            AreaControl areaControl = m_AreaControls[i];
            areaControl.OnEndTurn(this);
        }
    }

    private List<(int, int)> TunnelBetween(int x1, int y1, int x2, int y2)
    {
        List<(int, int)> tunnel = new List<(int, int)>();

        int cornerX = x2;
        int cornerY = y1;

        if (UnityEngine.Random.value < 0.5f)
        {
            cornerX = x1;
            cornerY = y2;
        }

        Bresenham.Line(x1, y1, cornerX, cornerY, (x, y) => { tunnel.Add((x, y)); return true; });
        Bresenham.Line(cornerX, cornerY, x2, y2, (x, y) => { tunnel.Add((x, y)); return true; });

        return tunnel;
    }

    private void SpawnEntities(RectangularRoom room)
    {
        for (int i = room.X1 + 1; i < room.X2; ++i)
        {
            for (int j = room.Y1 + 1; j < room.Y2; ++j)
            {
                if (m_Tiles[i][j] != null)
                {
                    continue;
                }
                AddTile(Floor, i, j, false, true);
            }
        }

        for (int i = room.Y1; i < room.Y2 + 1; ++i)
        {
            AddTile(Wall, room.X1, i, true, true);
            AddTile(Wall, room.X2, i, true, true);
        }

        for (int i = room.X1 + 1; i < room.X2; ++i)
        {
            AddTile(Wall, i, room.Y1, true, true);
            AddTile(Wall, i, room.Y2, true, true);
        }

        int nbMobs = UnityEngine.Random.Range(m_FloorDefinition.MinMobsPerRoom, m_FloorDefinition.MaxMobsPerRoom + 1);
        for (int i = 0; i < nbMobs; ++i)
        {
            int x = UnityEngine.Random.Range(room.X1 + 1, room.X2);
            int y = UnityEngine.Random.Range(room.Y1 + 1, room.Y2);

            GameObject mob = m_FloorDefinition.GetMob();
            if (mob != null)
            {
                AddActor(mob, x, y);
            }
        }

        int nbItems = UnityEngine.Random.Range(m_FloorDefinition.MinItemsPerRoom, m_FloorDefinition.MaxItemsPerRoom + 1);
        for (int i = 0; i < nbItems; ++i)
        {
            int x = UnityEngine.Random.Range(room.X1 + 1, room.X2);
            int y = UnityEngine.Random.Range(room.Y1 + 1, room.Y2);

            GameObject item = m_FloorDefinition.GetItem();
            if (item != null)
            {
                AddActor(item, x, y);
            }
        }
    }

    private void SpawnTunnel(List<(int, int)> tunnel)
    {
        foreach ((int, int) tile in tunnel)
        {
            AddTile(Floor, tile.Item1, tile.Item2, true, false);
        }
    }

    private void SpawnTunnelWalls(List<(int, int)> tunnel)
    {
        foreach ((int, int) tile in tunnel)
        {
            AddTile(Wall, tile.Item1 - 1, tile.Item2 + 1, false, false);
            AddTile(Wall, tile.Item1 - 1, tile.Item2, false, false);
            AddTile(Wall, tile.Item1 - 1, tile.Item2 - 1, false, false);
            AddTile(Wall, tile.Item1, tile.Item2 - 1, false, false);
            AddTile(Wall, tile.Item1, tile.Item2 + 1, false, false);
            AddTile(Wall, tile.Item1 + 1, tile.Item2 + 1, false, false);
            AddTile(Wall, tile.Item1 + 1, tile.Item2, false, false);
            AddTile(Wall, tile.Item1 + 1, tile.Item2 - 1, false, false);
        }
    }

    private void SpawnStairs(RectangularRoom room)
    {
        while (true)
        {
            int x = UnityEngine.Random.Range(room.X1 + 1, room.X2);
            int y = UnityEngine.Random.Range(room.Y1 + 1, room.Y2);

            if (GetActorAtLocation(x, y) == null)
            {
                Tile tile = GetTileAtLocation(x, y);
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                    m_Tiles[x][y] = null;
                }

                GameObject stairs = Instantiate(Stairs, new Vector3((float)x, (float)y, 0.0f), Quaternion.identity);
                m_Stairs = stairs.GetComponent<Tile>();
                m_Tiles[x][y] = m_Stairs;
                break;
            }
        }
    }

    private void AddTile(GameObject prefab, int x, int y, bool destroy, bool canBeDestroyed)
    {
        if (destroy && !m_CantDestroy.Contains((x, y)))
        {
            Tile tileToDestroy = m_Tiles[x][y];
            if (tileToDestroy != null)
            {
                Destroy(tileToDestroy.gameObject);
                m_Tiles[x][y] = null;
            }
        }

        if (m_Tiles[x][y] == null)
        {
            GameObject gameObject = Instantiate(prefab, new Vector3((float)x, (float)y, 0.0f), Quaternion.identity);
            gameObject.transform.parent = transform;
            m_Tiles[x][y] = gameObject.GetComponent<Tile>();
        }

        if (!canBeDestroyed)
        {
            m_CantDestroy.Add((x, y));
        }
    }

    private void AddActor(GameObject prefab, int x, int y)
    {
        if (GetActorAtLocation(x, y) == null)
        {
            GameObject gameObject = Instantiate(prefab, new Vector3((float)x, (float)y, 0.0f), Quaternion.identity);
            Tile tile = gameObject.GetComponent<Tile>();
            tile.X = x;
            tile.Y = y;
            m_Actors.Add(tile);
        }
    }

    private void ForEachTile(Action<Tile> action)
    {
        foreach (List<Tile> column in m_Tiles)
        {
            foreach (Tile tile in column)
            {
                if (tile != null)
                {
                    action(tile);
                }
            }
        }
    }

    private void ForEachActor(Action<Tile> action)
    {
        foreach (Tile actor in m_Actors)
        {
            action(actor);
        }
    }
}
