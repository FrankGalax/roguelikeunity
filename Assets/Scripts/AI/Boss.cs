using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Stair m_Stairs;

    private void Start()
    {
        GameMap gameMap = FindObjectOfType<GameMap>();
        m_Stairs = gameMap.GetStairs().GetComponent<Stair>();
        m_Stairs.Block();
    }

    private void OnDestroy()
    {
        m_Stairs.UnBlock();
    }
}
