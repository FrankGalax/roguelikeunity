﻿using UnityEngine;
using System.Collections.Generic;

public class SpellInstance
{
    public SpellTargetType SpellTargetType { get; set; }
    public List<SpellEffect> SpellEffects { get; set; }
    public int Radius { get; set; }

    public void Cast(GameObject gameObject, GameMap gameMap)
    {
        switch (SpellTargetType)
        {
            case SpellTargetType.RandomEnemy:
                CastRandomEnemy(gameObject, gameMap);
                break;
            case SpellTargetType.SingleTarget:
                CastSingleTarget(gameObject, gameMap);
                break;
            case SpellTargetType.AreaTarget:
                CastAreaTarget(gameObject, gameMap);
                break;
            case SpellTargetType.DirectionalTarget:
                CastDirectionalTarget(gameObject, gameMap);
                break;
        }
    }

    private void CastRandomEnemy(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseNoTargetSpellAction
        {
            GameObject = gameObject,
            SpellCallback = RandomEnemyUseSpell
        });
    }

    private void CastSingleTarget(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseSingleTargetSpellAction
        {
            GameObject = gameObject,
            SpellCallback = SingleTargetUseSpell
        });
    }

    private void CastAreaTarget(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseAreaTargetSpellAction
        {
            GameObject = gameObject,
            Radius = Radius,
            SpellCallback = AreaTargetUseSpell
        });
    }

    private void CastDirectionalTarget(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseDirectionalTargetSpellAction
        {
            GameObject = gameObject,
            SpellCallback = DirectionalTargetUseSpell
        });
    }

    private bool RandomEnemyUseSpell(GameObject gameObject, GameMap gameMap)
    {
        List<Tile> visibleEnemies = gameMap.GetVisibleEnemies();

        if (visibleEnemies.Count == 0)
        {
            return false;
        }

        int r = UnityEngine.Random.Range(0, visibleEnemies.Count);
        Tile visibleEnemy = visibleEnemies[r];

        TargetActor(gameObject, visibleEnemy);

        Tile tile = gameMap.GetTileAtLocation(visibleEnemy.X, visibleEnemy.Y);
        if (tile != null)
        {
            TargetTile(gameObject, tile);
        }

        return true;
    }
    private bool SingleTargetUseSpell(GameObject gameObject, GameMap gameMap, Tile target)
    {
        if (target == null)
        {
            return false;
        }

        TargetActor(gameObject, target);

        Tile tile = gameMap.GetTileAtLocation(target.X, target.Y);
        if (tile != null)
        {
            TargetTile(gameObject, tile);
        }

        return true;
    }

    private bool AreaTargetUseSpell(GameObject gameObject, GameMap gameMap, Tile target)
    {
        List<Tile> enemies = gameMap.GetEnemiesInRange(target.X, target.Y, Radius);
        List<Tile> floors = gameMap.GetFloorsInRange(target.X, target.Y, Radius);

        foreach (Tile enemy in enemies)
        {
            TargetActor(gameObject, enemy);
        }

        foreach (Tile floor in floors)
        {
            TargetTile(gameObject, floor);
        }

        return true;
    }

    private bool DirectionalTargetUseSpell(GameObject gameObject, GameMap gameMap, (int, int) direction)
    {
        Tile tile = gameObject.GetComponent<Tile>();
        for (int i = 1; i <= Radius; ++i)
        {
            int destX = tile.X + direction.Item1 * i;
            int destY = tile.Y + direction.Item2 * i;

            Tile gameMapTile = gameMap.GetTileAtLocation(destX, destY);
            if (gameMapTile == null)
            {
                break;
            }

            if (gameMapTile.BlocksMovement)
            {
                break;
            }

            Tile actor = gameMap.GetActorAtLocation(destX, destY);
            if (actor != null)
            {
                TargetActor(gameObject, actor);
            }

            TargetTile(gameObject, gameMapTile);
        }

        return true;
    }

    private void TargetActor(GameObject gameObject, Tile actor)
    {
        foreach (SpellEffect spellEffect in SpellEffects)
        {
            spellEffect.TargetActor(gameObject, actor);
        }
    }

    private void TargetTile(GameObject gameObject, Tile tile)
    {
        foreach (SpellEffect spellEffect in SpellEffects)
        {
            spellEffect.TargetTile(gameObject, tile, Radius);
        }
    }
}