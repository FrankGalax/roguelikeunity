using UnityEngine;

public class MeleeAction : GameAction
{
    public GameObject Target { get; set; }

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        DamageComponent damageComponent = Target.GetComponent<DamageComponent>();
        if (damageComponent == null)
        {
            IsDone = true;
            return;
        }

        MeleeComponent meleeComponent = GameObject.GetComponent<MeleeComponent>();
        if (meleeComponent == null)
        {
            IsDone = true;
            return;
        }

        damageComponent.TakeDamage(GameObject, meleeComponent.Attack);
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        IsDone = true;
    }
}