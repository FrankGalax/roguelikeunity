using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewConfusionSpellItemEffect", menuName = "Items/ConfusionSpellItemEffect")]
public class ConfusionSpellItemEffect : ItemEffect
{
    public int NbTurns = 5;

    private GameObject m_GameObject;

    public override void Apply(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseSingleTargetSpellAction { GameObject = gameObject, SpellCallback = UseSpell });
    }

    private bool UseSpell(GameMap gameMap, Tile target)
    {
        if (target == null)
        {
            return false;
        }

        AIComponent aiComponent = target.GetComponent<AIComponent>();
        if (aiComponent == null)
        {
            return false;
        }

        aiComponent.Confuse(NbTurns);

        return true;
    }
}