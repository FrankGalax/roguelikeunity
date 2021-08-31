using UnityEngine;

public class PassAction : GameAction
{
    public float WaitTime = 0.1f;

    private float m_Timer;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

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
}
