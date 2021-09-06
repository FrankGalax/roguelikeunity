using UnityEngine;

public class RangeAttackAIState : AIState
{
    public override GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        return new RangeAttackAction { GameObject = gameObject, Target = player };
    }
}