using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewFireballSpellItemEffect", menuName = "Items/FireballSpellItemEffect")]
public class FireballSpellItemEffect : ItemEffect
{
    public int Damage;
    public int Radius;
    private GameObject m_GameObject;

    public override void Apply(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseAreaTargetSpellAction { GameObject = gameObject, Radius = Radius, SpellCallback = UseSpell });
    }

    private bool UseSpell(GameMap gameMap, Tile target)
    {
        List<Tile> enemies = gameMap.GetEnemiesInRange(target.X, target.Y, Radius);

        if (enemies.Count == 0)
        {
            return false;
        }

        foreach (Tile enemy in enemies)
        {
            enemy.GetComponent<DamageComponent>().TakeDamage(m_GameObject, Damage);
        }

        return true;
    }
}