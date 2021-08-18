using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Room
{
    public Room(int x, int y, int width, int height)
    {
        X1 = x;
        Y1 = y;
        X2 = x + width;
        Y2 = y + height;
    }

    public bool Intersects(Room otherRoom)
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

public class GameMap : MonoBehaviour
{
    public GameObject Player;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject Rat;
    public GameObject Troll;
    public GameObject HealthPotion;
    public int MaxRooms;
    public int RoomMinSize;
    public int RoomMaxSize;
    public int DungeonWidth;
    public int DungeonHeight;
    public int MaxMobsPerRoom;
    public int MaxItemsPerRoom;

    private List<List<Tile>> m_Tiles;
    private HashSet<(int, int)> m_CantDestroy;
    private List<Tile> m_Actors;

    private void Awake()
    {
        m_Tiles = new List<List<Tile>>();
        m_Actors = new List<Tile>();
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
        List<Room> rooms = new List<Room>();
        List<List<(int, int)>> tunnels = new List<List<(int, int)>>();
        int playerX = 0;
        int playerY = 0;

        for (int i = 0; i < MaxRooms; ++i)
        {
            int roomWidth = UnityEngine.Random.Range(RoomMinSize, RoomMaxSize + 1);
            int roomHeight = UnityEngine.Random.Range(RoomMinSize, RoomMaxSize + 1);

            int x = UnityEngine.Random.Range(0, DungeonWidth - roomWidth);
            int y = UnityEngine.Random.Range(0, DungeonHeight - roomHeight);

            Room newRoom = new Room(x, y, roomWidth, roomHeight);

            bool intersect = false;
            foreach (Room otherRoom in rooms)
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
                Player.transform.position = new Vector3((float)roomCenter.Item1, (float)roomCenter.Item2, 0.0f);
                Tile playerTile = Player.GetComponent<Tile>();
                playerTile.X = roomCenter.Item1;
                playerTile.Y = roomCenter.Item2;
                Camera.main.transform.position = new Vector3(
                    Player.transform.position.x, 
                    Player.transform.position.y, 
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

            GameAction action = aiComponent.GetAction(this, Player);
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

    private void SpawnEntities(Room room)
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

        int nbMobs = UnityEngine.Random.Range(0, MaxMobsPerRoom + 1);
        for (int i = 0; i < nbMobs; ++i)
        {
            int x = UnityEngine.Random.Range(room.X1 + 1, room.X2);
            int y = UnityEngine.Random.Range(room.Y1 + 1, room.Y2);

            float r = UnityEngine.Random.value;
            if (r < 0.8)
            {
                AddActor(Rat, x, y);
            }
            else
            {
                AddActor(Troll, x, y);
            }
        }

        int nbItems = UnityEngine.Random.Range(0, MaxItemsPerRoom + 1);
        for (int i = 0; i < nbItems; ++i)
        {
            int x = UnityEngine.Random.Range(room.X1 + 1, room.X2);
            int y = UnityEngine.Random.Range(room.Y1 + 1, room.Y2);

            AddActor(HealthPotion, x, y);
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
