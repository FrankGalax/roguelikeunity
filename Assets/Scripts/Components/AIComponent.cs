using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
    public float Speed = 4.0f;

    public GameAction GetAction(GameMap gameMap, GameObject player)
    {
        return new GoToPlayerAction { GameObject = gameObject, Player = player, Speed = Speed };
    }
}
