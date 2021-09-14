using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageSpellEffect", menuName = "Spells/DamageSpellEffect")]
public class DamageSpellEffect : SpellEffect
{
    public int Damage;
    public DamageType DamageType;

    public override void TargetActor(GameObject gameObject, Tile actor)
    {
        base.TargetActor(gameObject, actor);

        DamageComponent damageComponent = actor.GetComponent<DamageComponent>();
        if (damageComponent != null)
        {
            damageComponent.TakeDamage(gameObject, Damage, DamageType);
        }
    }
}