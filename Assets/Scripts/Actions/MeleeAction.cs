using UnityEngine;

public class MeleeAction : GameAction
{
    public GameObject Target { get; set; }
    public float WaitTime = 0.5f;
    public float AIAttackTime = 0.25f;

    private float m_Timer;
    private float m_AIAttackTimer;

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

        if (GameObject.CompareTag("Player"))
        {
            damageComponent.TakeDamage(GameObject, meleeComponent.Attack, DamageType.Physical);
            m_AIAttackTimer = -1.0f;
        }
        else
        {
            m_AIAttackTimer = AIAttackTime;
        }

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

        if (m_AIAttackTimer > 0.0f)
        {
            m_AIAttackTimer -= Time.deltaTime;
            if (m_AIAttackTimer <= 0.0f)
            {
                m_AIAttackTimer = -1.0f;
                DamageComponent damageComponent = Target.GetComponent<DamageComponent>();
                MeleeComponent meleeComponent = GameObject.GetComponent<MeleeComponent>();
                damageComponent.TakeDamage(GameObject, meleeComponent.Attack, DamageType.Physical);
            }
        }
    }

    public override string GetDebugString()
    {
        return "MeleeAction with Target " + (Target != null ? Target.name : "NULL");
    }
}