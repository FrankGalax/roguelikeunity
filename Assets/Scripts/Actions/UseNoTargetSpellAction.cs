using System;
using UnityEngine;

public class UseNoTargetSpellAction : GameAction
{
    public float WaitTime = 0.5f;
    public Func<GameObject, GameMap, bool> SpellCallback { get; set; }

    private float m_Timer;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        IsSuccess = SpellCallback(GameObject, gameMap);

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
        return "UseNoTargetSpellAction";
    }
}
