using UnityEngine;

public class LearnSpellReward : Reward
{
    public Spell Spell { get; set; }

    public override void Apply(GameObject player)
    {
        SpellComponent spellComponent = player.GetComponent<SpellComponent>();
        spellComponent.LearnSpell(Spell);
    }

    public override Sprite Icon { get { return Spell.Icon; } }

    public override string Name { get { return "Learn " + Spell.Name; } }
}