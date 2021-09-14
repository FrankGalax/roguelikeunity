using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellItemEffect", menuName = "Items/SpellItemEffect")]
public class SpellItemEffect : ItemEffect
{
    public Spell Spell;

    public override void Apply(GameObject gameObject, GameMap gameMap)
    {
        Spell.Cast(gameObject, gameMap);
    }
}