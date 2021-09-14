using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectSpellEffect", menuName = "Spells/EffectSpellEffect")]
public class EffectSpellEffect : SpellEffect
{
    public Effect Effect;
    public int NbTurns;

    public override void TargetActor(GameObject gameObject, Tile actor)
    {
        base.TargetActor(gameObject, actor);

        EffectComponent effectComponent = actor.GetComponent<EffectComponent>();
        if (effectComponent != null)
        {
            effectComponent.AddEffect(Effect, NbTurns);
        }
    }
}
