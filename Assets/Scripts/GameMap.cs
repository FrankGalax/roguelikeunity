using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

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
    public int DungeonWidth;
    public int DungeonHeight;

    private List<List<Tile>> m_Tiles;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        int playerX = 0;
        int playerY = 0;

        int currentFloor = GameManager.Instance.CurrentFloor;
        if (currentFloor >= Config.Instance.FloorDefinitions.Count)
        {
            m_FloorDefinition = Config.Instance.FloorDefinitions[Config.Instance.FloorDefinitions.Count - 1];
        }
        else
        {
            m_FloorDefinition = Config.Instance.FloorDefinitions[currentFloor];
        }

        List<List<char>> dungeon = BrogueGen.GenerateDungeon(DungeonWidth, DungeonHeight, m_FloorDefinition);

        for (int i = 0; i < DungeonWidth; ++i)
        {
            for (int j = 0; j < DungeonHeight; ++j)
            {
                switch (dungeon[i][j])
                {
                    case '#':
                        AddTile(Wall, i, j);
                        break;
                    case '.':
                        AddTile(Floor, i, j);
                        break;
                    case 'd':
                        AddTile(Door, i, j);
                        break;
                    case 's':
                        {
                            AddTile(Stairs, i, j);
                            m_Stairs = m_Tiles[i][j];
                            break;
                        }
                    case 'p':
                        {
                            AddTile(Floor, i, j);

                            m_Player.transform.position = new Vector3((float)i, (float)j, 0.0f);
                            Tile playerTile = m_Player.GetComponent<Tile>();
                            playerTile.X = i;
                            playerTile.Y = j;
                            Camera.main.transform.position = new Vector3(
                                m_Player.transform.position.x,
                                m_Player.transform.position.y,
                                Camera.main.transform.position.z
                            );
                            playerX = i;
                            playerY = j;

                            break;
                        }
                    case 'm':
                        {
                            AddTile(Floor, i, j);
                            AddActor(m_FloorDefinition.GetMob(), i, j);
                            break;
                        }
                    case 'i':
                        {
                            AddTile(Floor, i, j);
                            AddActor(m_FloorDefinition.GetItem(), i, j);
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

        foreach (Tile actor in m_Actors)
        {
            EffectComponent effectComponent = actor.GetComponent<EffectComponent>();
            if (effectComponent != null)
            {
                effectComponent.EndTurn();
            }
        }

        EffectComponent playerEffectComponent = m_Player.GetComponent<EffectComponent>();
        if (playerEffectComponent != null)
        {
            playerEffectComponent.EndTurn();
        }
    }

    public void AddTile(GameObject prefab, int x, int y)
    {
        if (prefab == null)
        {
            return;
        }

        Assert.IsNull(m_Tiles[x][y]);
        GameObject gameObject = Instantiate(prefab, new Vector3((float)x, (float)y, 0.0f), Quaternion.identity);
        gameObject.transform.parent = transform;
        m_Tiles[x][y] = gameObject.GetComponent<Tile>();
    }

    public void RemoveTile(int x, int y)
    {
        Tile tile = GetTileAtLocation(x, y);
        if (tile != null)
        {
            Destroy(tile.gameObject);
            m_Tiles[x][y] = null;
        }
    }

    private void AddActor(GameObject prefab, int x, int y)
    {
        if (prefab == null)
        {
            return;
        }

        Assert.IsNull(GetActorAtLocation(x, y));
        GameObject gameObject = Instantiate(prefab, new Vector3((float)x, (float)y, 0.0f), Quaternion.identity);
        Tile tile = gameObject.GetComponent<Tile>();
        tile.X = x;
        tile.Y = y;
        m_Actors.Add(tile);
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

    public void CheatShowAllTiles()
    {
        ForEachTile((tile) => 
        { 
            tile.IsVisible = true;
            tile.IsDiscovered = true;
            tile.UpdateVisibility();
            tile.AlwaysVisible = true;
        });
    }

    public void CheatSpawn(string name)
    {
        Tile playerTile = m_Player.GetComponent<Tile>();
        foreach (FloorDefinition floorDefinition in Config.Instance.FloorDefinitions)
        {
            foreach (Spawnable mob in floorDefinition.Mobs)
            {
                if (mob.GameObject.name.ToLower().Contains(name.ToLower()))
                {
                    AddActor(mob.GameObject, playerTile.X, playerTile.Y + 2);
                    ComputeFOV(playerTile.X, playerTile.Y);
                    return;
                }
            }

            foreach (Spawnable item in floorDefinition.Items)
            {
                if (item.GameObject.name.ToLower().Contains(name.ToLower()))
                {
                    AddActor(item.GameObject, playerTile.X + 1, playerTile.Y);
                    ComputeFOV(playerTile.X, playerTile.Y);
                    return;
                }
            }
        }
    }
}
