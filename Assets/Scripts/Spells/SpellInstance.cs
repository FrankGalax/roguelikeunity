using UnityEngine;
using System.Collections.Generic;

public class SpellInstance
{
    public SpellTargetType SpellTargetType { get; set; }
    public List<SpellEffect> SpellEffects { get; set; }
    public int Radius { get; set; }
    public Spell Spell { get; set; }
    public Item Item { get; set; }

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
            case SpellTargetType.Self:
                CastSelf(gameObject, gameMap);
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
            SpellCallback = SingleTargetUseSpell,
            SpellCanceledCallback = CancelSpell
        });
    }

    private void CastAreaTarget(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseAreaTargetSpellAction
        {
            GameObject = gameObject,
            Radius = Radius,
            SpellCallback = AreaTargetUseSpell,
            SpellCanceledCallback = CancelSpell
        });
    }

    private void CastDirectionalTarget(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseDirectionalTargetSpellAction
        {
            GameObject = gameObject,
            SpellCallback = DirectionalTargetUseSpell,
            SpellCanceledCallback = CancelSpell
        });
    }

    private void CastSelf(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseNoTargetSpellAction
        {
            GameObject = gameObject,
            SpellCallback = SelfUseSpell
        });
    }

    private bool RandomEnemyUseSpell(GameObject gameObject, GameMap gameMap)
    {
        List<Tile> visibleEnemies = gameMap.GetVisibleEnemies();

        bool isCanceled = false;
        if (visibleEnemies.Count == 0)
        {
            ReleaseSpell(gameObject, isCanceled);
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

        ReleaseSpell(gameObject, isCanceled);
        return true;
    }

    private bool SingleTargetUseSpell(GameObject gameObject, GameMap gameMap, Tile target)
    {
        bool isCanceled = false;
        if (target == null)
        {
            ReleaseSpell(gameObject, isCanceled);
            return false;
        }

        TargetActor(gameObject, target);

        Tile tile = gameMap.GetTileAtLocation(target.X, target.Y);
        if (tile != null)
        {
            TargetTile(gameObject, tile);
        }

        ReleaseSpell(gameObject, isCanceled);
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

        bool isCanceled = false;
        ReleaseSpell(gameObject, isCanceled);
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

        bool isCanceled = false;
        ReleaseSpell(gameObject, isCanceled);
        return true;
    }
    private bool SelfUseSpell(GameObject gameObject, GameMap gameMap)
    {
        TargetActor(gameObject, gameObject.GetComponent<Tile>());

        bool isCanceled = false;
        ReleaseSpell(gameObject, isCanceled);
        return true;
    }

    private void CancelSpell(GameObject gameObject)
    {
        bool isCanceled = true;
        ReleaseSpell(gameObject, isCanceled);
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

    private void ReleaseSpell(GameObject gameObject, bool isCanceled)
    {
        if (Item != null)
        {
            InventoryComponent inventoryComponent = gameObject.GetComponent<InventoryComponent>();
            if (inventoryComponent != null)
            {
                inventoryComponent.StopUsingItem(Item, isCanceled);
            }
        }
    }
}