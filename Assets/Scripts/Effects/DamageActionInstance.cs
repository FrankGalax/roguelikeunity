using UnityEngine;

public class DamageActionInstance : GameplayActionInstance
{
    private DamageAction m_DamageAction;

    public override void InitAction(GameObject gameObject, GameObject instigator)
    {
        base.InitAction(gameObject, instigator);

        m_DamageAction = (DamageAction)GameplayAction;
    }

    public override void EndTurn(GameObject gameObject, int turnCount)
    {
        base.EndTurn(gameObject, turnCount);

        DamageComponent damageComponent = gameObject.GetComponent<DamageComponent>();
        if (damageComponent != null)
        {
            damageComponent.TakeDamage(m_Instigator, m_DamageAction.Damage, m_DamageAction.DamageType);
        }
    }
}