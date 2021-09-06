using UnityEngine;

public class GoToPlayerAIState : AIState
{
    public override GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        GameAction effectGameAction = AIHelper.GetEffectGameplayAction(gameObject, player);
        if (effectGameAction != null)
        {
            return effectGameAction;
        }

        return new GoToPlayerAction { GameObject = gameObject, Player = player };
    }
}