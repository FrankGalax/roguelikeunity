using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellItemEffect", menuName = "Items/SpellItemEffect")]
public class SpellItemEffect : ItemEffect
{
    public Spell Spell;

    public override void Apply(GameObject gameObject, GameMap gameMap, Item item)
    {
        SpellInstance spellInstance = Spell.CreateInstance();
        spellInstance.Item = item;
        spellInstance.Cast(gameObject, gameMap);
    }
}