using UnityEngine;
using System;

public class UseDirectionalTargetSpellAction : GameAction
{
    public float WaitTime = 0.5f;
    public Func<GameObject, GameMap, (int, int), bool> SpellCallback { get; set; }
    public Action<GameObject> SpellCanceledCallback { get; set; }

    private float m_Timer;
    private TargetComponent m_TargetComponent;
    private bool m_SpellCallbackCalled;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        GameManager.Instance.RequestGameState(GameStateRequest.DirectionalTarget);
        m_TargetComponent = GameObject.GetComponent<TargetComponent>();
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
        else if (m_TargetComponent.Direction != null)
        {
            m_Timer = WaitTime;
            IsSuccess = SpellCallback(GameObject, gameMap, m_TargetComponent.Direction.Value);
            GameManager.Instance.RequestGameState(GameStateRequest.Dungeon);
            m_SpellCallbackCalled = true;
        }
        else if (m_TargetComponent.IsCanceled)
        {
            IsDone = true;
            IsSuccess = false;
            GameManager.Instance.RequestGameState(GameStateRequest.Dungeon);
            SpellCanceledCallback(GameObject);
        }
    }

    public override string GetDebugString()
    {
        return "UseDirectionalTargetSpellAction";
    }
}