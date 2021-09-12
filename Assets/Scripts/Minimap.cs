using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Player;
    public GameObject Door;

    private RectTransform m_RectTransform;
    private int m_DungeonWidth;
    private int m_DungeonHeight;
    private List<List<GameObject>> m_MinimapTiles;
    private Tile m_PlayerTile;
    private GameObject m_PlayerMinimapTile;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerTile = player.GetComponent<Tile>();
    }

    private void Update()
    {
        if (m_PlayerMinimapTile != null)
        {
            RectTransform rectTransform = m_PlayerMinimapTile.GetComponent<RectTransform>();
            float x = (float)m_PlayerTile.X / (float)m_DungeonWidth * m_RectTransform.rect.width;
            float y = (float)m_PlayerTile.Y / (float)m_DungeonHeight * m_RectTransform.rect.height;
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    public void OnDungeonCreated(List<List<char>> dungeon)
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_DungeonWidth = dungeon.Count;
        m_DungeonHeight = dungeon[0].Count;

        m_MinimapTiles = new List<List<GameObject>>();

        for (int i = 0; i < m_DungeonWidth; ++i)
        {
            List<GameObject> column = new List<GameObject>();

            for (int j = 0; j < m_DungeonHeight; ++j)
            {
                column.Add(null);

                switch (dungeon[i][j])
                {
                    case '#':
                        column[j] = AddTile(Wall, i, j);
                        column[j].SetActive(false);
                        break;
                    case 'd':
                        column[j] = AddTile(Door, i, j);
                        column[j].SetActive(false);
                        break;
                }
            }

            m_MinimapTiles.Add(column);
        }

        m_PlayerMinimapTile = AddTile(Player, m_PlayerTile.X, m_PlayerTile.Y);
    }

    public void UpdateVisibility(List<List<Tile>> tiles)
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            for (int j = 0; j < tiles[i].Count; ++j)
            {
                if (tiles[i][j] != null && m_MinimapTiles[i][j] != null)
                {
                    m_MinimapTiles[i][j].SetActive(tiles[i][j].IsDiscovered);
                }
            }
        }
    }

    public void RemoveTile(Tile tile, int x, int y)
    {
        if (tile.GetComponent<DoorComponent>() != null)
        {
            Destroy(m_MinimapTiles[x][y]);
            m_MinimapTiles[x][y] = null;
        }
    }

    private GameObject AddTile(GameObject prefab, int i, int j)
    {
        GameObject gameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        float x = (float)i / (float)m_DungeonWidth * m_RectTransform.rect.width;
        float y = (float)j / (float)m_DungeonHeight * m_RectTransform.rect.height;
        float sizeX = m_RectTransform.rect.width / (float)m_DungeonWidth;
        float sizeY = m_RectTransform.rect.height / (float)m_DungeonHeight;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x, y);
        rectTransform.sizeDelta = new Vector2(sizeX, sizeY);

        return gameObject;
    }
}
