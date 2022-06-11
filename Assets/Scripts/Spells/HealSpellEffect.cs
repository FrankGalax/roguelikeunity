using UnityEngine;

[CreateAssetMenu(fileName = "NewHealSpellEffect", menuName = "Spells/HealSpellEffect")]
public class HealSpellEffect : SpellEffect
{
    public int Life;

    public override void TargetActor(GameObject gameObject, Tile actor)
    {
        base.TargetActor(gameObject, actor);

        DamageComponent damageComponent = actor.GetComponent<DamageComponent>();
        if (damageComponent != null)
        {
            damageComponent.Heal(Life);
        }
    }
}