using UnityEngine;

public class MeleeAction : GameAction
{
    public GameObject Target { get; set; }
    public float WaitTime = 0.5f;

    private float m_Timer;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        DamageComponent damageComponent = Target.GetComponent<DamageComponent>();
        if (damageComponent == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        MeleeComponent meleeComponent = GameObject.GetComponent<MeleeComponent>();
        if (meleeComponent == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        damageComponent.TakeDamage(GameObject, meleeComponent.Attack);
        m_Timer = WaitTime;
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
        {
            IsDone = true;
        }
    }

    public override string GetDebugString()
    {
        return "MeleeAction with Target " + (Target != null ? Target.name : "NULL");
    }
}