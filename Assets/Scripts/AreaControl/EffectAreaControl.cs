using UnityEngine;

public class EffectAreaControl : AreaControl
{
    public int EffectNbTurns;
    public Effect Effect;

    public override void OnEnter(GameObject gameObject)
    {
        base.OnEnter(gameObject);

        EffectComponent effectComponent = gameObject.GetComponent<EffectComponent>();
        if (effectComponent != null)
        {
            effectComponent.AddEffect(Effect, EffectNbTurns);
        }
    }
}