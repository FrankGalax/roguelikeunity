using UnityEngine;

public class PassAIState : AIState
{
    public override GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        return new PassAction { GameObject = gameObject };
    }
}
