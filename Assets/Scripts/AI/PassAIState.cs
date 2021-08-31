using UnityEngine;

[CreateAssetMenu(fileName = "NewPassAIState", menuName = "AI/PassAIState")]
public class PassAIState : AIState
{
    public override GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap)
    {
        return new PassAction { GameObject = gameObject };
    }
}
