using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGainManaSpellEffect", menuName = "Spells/GainManaSpellEffect")]
public class GainManaSpellEffect : SpellEffect
{
    public int Mana;

    public override void TargetActor(GameObject gameObject, Tile actor)
    {
        base.TargetActor(gameObject, actor);

        SpellComponent spellComponent = actor.GetComponent<SpellComponent>();
        if (spellComponent != null)
        {
            spellComponent.GainMana(Mana);
        }
    }
}
