using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public abstract GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap);
}