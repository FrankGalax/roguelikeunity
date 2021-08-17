using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
    public float Speed = 4.0f;

    private DamageComponent m_DamageComponent;

    private void Awake()
    {
        m_DamageComponent = GetComponent<DamageComponent>();
    }

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        if (!m_DamageComponent.IsAlive)
        {
            return null;
        }

        return new GoToPlayerAction { GameObject = gameObject, Player = player, Speed = Speed };
    }
}
