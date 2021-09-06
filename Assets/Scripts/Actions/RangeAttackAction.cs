using UnityEngine;

public class RangeAttackAction : GameAction
{
    public GameObject Target;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        RangeAttackComponent rangeAttackComponent = GameObject.GetComponent<RangeAttackComponent>();
        if (rangeAttackComponent == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        GameObject projectileGameObject = GameObject.Instantiate(rangeAttackComponent.Projectile, GameObject.transform.position, Quaternion.identity);
        ProjectileComponent projectile = projectileGameObject.GetComponent<ProjectileComponent>();
        projectile.Instigator = GameObject;
        projectile.Target = Target;
        projectile.Callback = OnProjectileHit;
    }

    private void OnProjectileHit()
    {
        IsDone = true;
    }
}