using UnityEngine;

public class ToggleAnimatorActionInstance : GameplayActionInstance
{
    private ToggleAnimatorAction m_ToggleAnimatorAction;
    private int m_Turns;
    private Animator m_Animator;
    private bool m_IsEnabled;

    public override void InitAction(GameObject gameObject, GameObject instigator)
    {
        base.InitAction(gameObject, instigator);

        m_ToggleAnimatorAction = (ToggleAnimatorAction)GameplayAction;
        AnimationComponent animationComponent = gameObject.GetComponent<AnimationComponent>();
        if (animationComponent == null)
        {
            animationComponent = gameObject.GetComponentInChildren<AnimationComponent>();
        }

        if (animationComponent != null)
        {
            m_Animator = animationComponent.GetComponent<Animator>();
        }
    }

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        if (m_Animator == null)
        {
            return;
        }

        Toggle(m_ToggleAnimatorAction.StartEnabled);
    }

    public override void StopAction(GameObject gameObject)
    {
        base.StopAction(gameObject);

        if (m_Animator == null)
        {
            return;
        }

        Toggle(true);
    }

    public override void EndTurn(GameObject gameObject, int turnCount)
    {
        base.EndTurn(gameObject, turnCount);

        if (m_Animator == null || turnCount <= 1)
        {
            return;
        }

        m_Turns++;

        if (m_IsEnabled)
        {
            if (m_Turns >= m_ToggleAnimatorAction.TurnsEnabled)
            {
                Toggle(false);
            }
        }
        else
        {
            if (m_Turns >= m_ToggleAnimatorAction.TurnsDisabled)
            {
                Toggle(true);
            }
        }
    }

    private void Toggle(bool enabled)
    {
        m_Animator.enabled = enabled;
        m_Turns = 0;
        m_IsEnabled = enabled;
    }
}