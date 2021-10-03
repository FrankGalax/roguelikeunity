using UnityEngine;

public class ToggleAIActionInstance : GameplayActionInstance
{
    private ToggleAIAction m_ToggleAIAction;
    private int m_Turns;
    private AIComponent m_AIComponent;
    private bool m_IsEnabled;

    public override void InitAction(GameObject gameObject)
    {
        base.InitAction(gameObject);

        m_ToggleAIAction = (ToggleAIAction)GameplayAction;
        m_AIComponent = gameObject.GetComponent<AIComponent>();
    }

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        if (m_AIComponent == null)
        {
            return;
        }

        Toggle(m_ToggleAIAction.StartEnabled);
    }

    public override void StopAction(GameObject gameObject)
    {
        base.StopAction(gameObject);

        if (m_AIComponent == null)
        {
            return;
        }

        Toggle(true);
    }

    public override void EndTurn(GameObject gameObject, int turnCount)
    {
        base.EndTurn(gameObject, turnCount);

        if (m_AIComponent == null || turnCount <= 1)
        {
            return;
        }

        m_Turns++;

        if (m_IsEnabled)
        {
            if (m_Turns >= m_ToggleAIAction.TurnsEnabled)
            {
                Toggle(false);
            }
        }
        else
        {
            if (m_Turns >= m_ToggleAIAction.TurnsDisabled)
            {
                Toggle(true);
            }
        }
    }

    private void Toggle(bool enabled)
    {
        m_AIComponent.enabled = enabled;
        m_Turns = 0;
        m_IsEnabled = enabled;
    }
}