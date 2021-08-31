using UnityEngine;

public class UnicornDustAreaControl : AreaControl
{
    public int SleepNbTurns;

    public override void OnEnter(GameObject gameObject)
    {
        base.OnEnter(gameObject);

        EffectComponent effectComponent = gameObject.GetComponent<EffectComponent>();
        if (effectComponent != null)
        {
            effectComponent.AddEffect(EffectType.Sleep, SleepNbTurns);
        }
    }
}