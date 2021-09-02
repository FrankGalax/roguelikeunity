﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLightningSpellItemEffect", menuName = "Items/LightningSpellItemEffect")]
public class LightningSpellItemEffect : ItemEffect
{
    public int Damage;
    public GameObject Lightning;
    private GameObject m_GameObject;

    public override void Apply(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseNoTargetSpellAction { GameObject = gameObject, SpellCallback = UseSpell });
    }

    private bool UseSpell(GameMap gameMap)
    {
        List<Tile> visibleEnemies = gameMap.GetVisibleEnemies();

        if (visibleEnemies.Count == 0)
        {
            return false;
        }

        int r = UnityEngine.Random.Range(0, visibleEnemies.Count);
        Tile visibleEnemy = visibleEnemies[r];

        visibleEnemy.GetComponent<DamageComponent>().TakeDamage(m_GameObject, Damage);
        Instantiate(Lightning, new Vector3((float)visibleEnemy.X, (float)visibleEnemy.Y, 0.0f), Quaternion.identity);

        return true;
    }
}