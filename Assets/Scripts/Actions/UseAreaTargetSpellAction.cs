using System;
using UnityEngine;

public class UseAreaTargetSpellAction : GameAction
{
    public float WaitTime = 0.5f;
    public Func<GameObject, GameMap, Tile, bool> SpellCallback { get; set; }
    public int Radius { get; set; }

    private float m_Timer;
    private TargetComponent m_TargetComponent;
    private bool m_SpellCallbackCalled;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        m_TargetComponent = GameObject.GetComponent<TargetComponent>();
        m_TargetComponent.Radius = Radius;
        GameManager.Instance.RequestGameState(GameStateRequest.AreaTarget);
        m_Timer = 0;
        m_SpellCallbackCalled = false;
        IsPausing = true;
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        if (m_SpellCallbackCalled)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0)
            {
                IsDone = true;
            }
        }
        else if (m_TargetComponent.TargetTile != null)
        {
            m_Timer = WaitTime;
            IsSuccess = SpellCallback(GameObject, gameMap, m_TargetComponent.TargetTile);
            GameManager.Instance.RequestGameState(GameStateRequest.Dungeon);
            m_SpellCallbackCalled = true;
        }
        else if (m_TargetComponent.IsCanceled)
        {
            IsDone = true;
            IsSuccess = false;
            GameManager.Instance.RequestGameState(GameStateRequest.Dungeon);
        }
    }

    public override string GetDebugString()
    {
        return "UseAreaTargetSpellAction";
    }
}
