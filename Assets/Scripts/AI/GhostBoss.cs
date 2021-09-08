using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBoss : MonoBehaviour
{
    public int SizeRadius = 2;
    public int TurnsVisible = 2;
    public int TurnsInvisible = 2;
    public int InvisibleDamage = 1;
    public int VisibleDamage = 2;
    public Color InvisibleColor;
    public Color VisibleColor;
    public Color MinionInvisibleColor;
    public Color MinionVisibleColor;
    public GameObject Minion;

    private (int, int) m_Direction;
    private bool m_IsVisible;
    private int m_TurnCount;
    private ColorChange m_ColorChange;
    private DamageComponent m_DamageComponent;
    private List<GameObject> m_Minions;
    private GameObject m_Player;

    private void Start()
    {
        m_ColorChange = GetComponentInChildren<ColorChange>();
        m_DamageComponent = GetComponent<DamageComponent>();
        AIComponent aiComponent = GetComponent<AIComponent>();
        aiComponent.GetActionFunc = GetAction;
        m_Player = GameObject.FindGameObjectWithTag("Player");

        int r = UnityEngine.Random.Range(0, 4);
        m_Direction = (0, 0);
        switch (r)
        {
            case 0:
                m_Direction = (-1, 1);
                break;
            case 1:
                m_Direction = (-1, -1);
                break;
            case 2:
                m_Direction = (1, -1);
                break;
            case 3:
                m_Direction = (1, 1);
                break;
        }

        m_IsVisible = true;
        m_TurnCount = 0;

        m_Minions = new List<GameObject>();
        int size = SizeRadius * 2 - 1;
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                GameObject minion = Instantiate(Minion, Vector3.zero, Quaternion.identity, transform);
                minion.transform.localPosition = new Vector3((float)1 - SizeRadius + i, (float)1 - SizeRadius + j, 0.0f);
                m_Minions.Add(minion);
            }
        }
    }

    private GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        Tile tile = gameObject.GetComponent<Tile>();

        Tile topTile = gameMap.GetTileAtLocation(tile.X, tile.Y + SizeRadius);
        Tile leftTile = gameMap.GetTileAtLocation(tile.X - SizeRadius, tile.Y);
        Tile bottomTile = gameMap.GetTileAtLocation(tile.X, tile.Y - SizeRadius);
        Tile rightTile = gameMap.GetTileAtLocation(tile.X + SizeRadius, tile.Y);

        bool hasCollisionTop = topTile == null || topTile.BlocksMovement;
        bool hasCollisionLeft = leftTile == null || leftTile.BlocksMovement;
        bool hasCollisionBottom = bottomTile == null || bottomTile.BlocksMovement;
        bool hasCollisionRight = rightTile == null || rightTile.BlocksMovement;

        if (hasCollisionTop || hasCollisionBottom)
        {
            m_Direction.Item2 = -m_Direction.Item2;
        }
        
        if (hasCollisionLeft || hasCollisionRight)
        {
            m_Direction.Item1 = -m_Direction.Item1;
        }

        m_TurnCount++;
        if (m_IsVisible)
        {
            if (m_TurnCount >= TurnsVisible)
            {
                m_TurnCount = 0;
                m_IsVisible = false;
                m_DamageComponent.IsInvulnerable = true;
                m_ColorChange.Target = InvisibleColor;
                foreach (GameObject minion in m_Minions)
                {
                    minion.GetComponentInChildren<ColorChange>().Target = MinionVisibleColor;
                }
            }
        }
        else
        {
            if (m_TurnCount >= TurnsInvisible)
            {
                m_TurnCount = 0;
                m_IsVisible = true;
                m_DamageComponent.IsInvulnerable = false;
                m_ColorChange.Target = VisibleColor;
                foreach (GameObject minion in m_Minions)
                {
                    minion.GetComponentInChildren<ColorChange>().Target = MinionInvisibleColor;
                }
            }
        }

        MoveAction moveAction = new MoveAction { GameObject = gameObject, Direction = m_Direction };
        moveAction.ReleaseSignal.AddSlot(OnMoveRelease);
        return moveAction;
    }

    private void OnMoveRelease()
    {
        Tile playerTile = m_Player.GetComponent<Tile>();
        Tile tile = GetComponent<Tile>();
        int radius = playerTile.GetRadius(tile.X, tile.Y);
        if (radius < SizeRadius)
        {
            int damage = m_IsVisible ? VisibleDamage : InvisibleDamage;
            m_Player.GetComponent<DamageComponent>().TakeDamage(gameObject, damage, DamageType.Cold);
        }
    }
}
