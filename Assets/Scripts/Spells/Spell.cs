using UnityEngine;
using System.Collections.Generic;

public enum SpellTargetType
{
    RandomEnemy,
    SingleTarget,
    AreaTarget
}

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public SpellTargetType SpellTargetType = SpellTargetType.SingleTarget;
    public List<SpellEffect> SpellEffects;
    public int Radius;

    private GameObject m_GameObject;

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
        }
    }

    private void CastRandomEnemy(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseNoTargetSpellAction 
        { 
            GameObject = gameObject, 
            SpellCallback = RandomEnemyUseSpell 
        });
    }

    private void CastSingleTarget(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseSingleTargetSpellAction 
        { 
            GameObject = gameObject, 
            SpellCallback = SingleTargetUseSpell 
        });
    }

    private void CastAreaTarget(GameObject gameObject, GameMap gameMap)
    {
        m_GameObject = gameObject;
        gameMap.GetComponent<ActionQueue>().AddAction(new UseAreaTargetSpellAction 
        { 
            GameObject = gameObject, 
            Radius = Radius, 
            SpellCallback = AreaTargetUseSpell 
        });
    }

    private bool RandomEnemyUseSpell(GameMap gameMap)
    {
        List<Tile> visibleEnemies = gameMap.GetVisibleEnemies();

        if (visibleEnemies.Count == 0)
        {
            return false;
        }

        int r = UnityEngine.Random.Range(0, visibleEnemies.Count);
        Tile visibleEnemy = visibleEnemies[r];

        TargetActor(m_GameObject, visibleEnemy);

        Tile tile = gameMap.GetTileAtLocation(visibleEnemy.X, visibleEnemy.Y);
        if (tile != null)
        {
            TargetTile(m_GameObject, tile);
        }

        return true;
    }
    private bool SingleTargetUseSpell(GameMap gameMap, Tile target)
    {
        if (target == null)
        {
            return false;
        }

        TargetActor(m_GameObject, target);

        Tile tile = gameMap.GetTileAtLocation(target.X, target.Y);
        if (tile != null)
        {
            TargetTile(m_GameObject, tile);
        }

        return true;
    }

    private bool AreaTargetUseSpell(GameMap gameMap, Tile target)
    {
        List<Tile> enemies = gameMap.GetEnemiesInRange(target.X, target.Y, Radius);
        List<Tile> floors = gameMap.GetFloorsInRange(target.X, target.Y, Radius);

        foreach (Tile enemy in enemies)
        {
            TargetActor(m_GameObject, enemy);
        }

        foreach (Tile floor in floors)
        {
            TargetTile(m_GameObject, floor);
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
            spellEffect.TargetTile(gameObject, tile);
        }
    }
}
