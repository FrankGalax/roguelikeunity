using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEffectSpellItemEffect", menuName = "Items/EffectSpellItemEffect")]
public class EffectSpellItemEffect : ItemEffect
{
    public int NbTurns = 5;
    public Effect Effect;

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

        EffectComponent effectComponent = target.GetComponent<EffectComponent>();
        if (effectComponent == null)
        {
            return false;
        }

        effectComponent.AddEffect(Effect, NbTurns);

        return true;
    }
}