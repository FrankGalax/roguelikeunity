using UnityEngine;
using System.Collections.Generic;

public class EffectInstance
{
    public Effect Effect { get; set; }
    public int RemainingTurns { get; set; }
    public bool HasDuration { get; set; }
    public List<GameplayActionInstance> GameplayActionInstances { get; set; }

    private int m_TurnCount;

    public EffectInstance(Effect effect, int remainingTurns)
    {
        Effect = effect;
        RemainingTurns = remainingTurns;
        HasDuration = remainingTurns >= 0;
        GameplayActionInstances = new List<GameplayActionInstance>();

        foreach (GameplayAction gameplayAction in effect.GameplayActions)
        {
            GameplayActionInstance instance = gameplayAction.CreateInstance();
            instance.GameplayAction = gameplayAction;
            GameplayActionInstances.Add(instance);
        }

        m_TurnCount = 0;
    }

    public void EndTurn(GameObject gameObject)
    {
        m_TurnCount++;

        foreach (GameplayActionInstance instance in GameplayActionInstances)
        {
            instance.EndTurn(gameObject, m_TurnCount);
        }
    }
}

public class EffectComponent : MonoBehaviour
{
    private List<EffectInstance> m_EffectInstances;

    private void Awake()
    {
        m_EffectInstances = new List<EffectInstance>();
    }

    public void AddEffect(Effect effect, int nbTurns)
    {
        foreach (EffectInstance effectInstance in m_EffectInstances)
        {
            if (effectInstance.Effect.EffectType == effect.EffectType)
            {
                effectInstance.RemainingTurns = nbTurns;
                return;
            }
        }

        EffectInstance newEffectInstance = new EffectInstance(effect, nbTurns);
        StartEffect(newEffectInstance);
        m_EffectInstances.Add(newEffectInstance);
    }

    public void RemoveEffect(Effect effect)
    {
        for (int i = 0; i < m_EffectInstances.Count; ++i)
        {
            if (m_EffectInstances[i].Effect == effect)
            {
                StopEffect(m_EffectInstances[i]);
                m_EffectInstances.RemoveAt(i);
                return;
            }
        }
    }

    public bool HasEffect(EffectType effectType)
    {
        foreach (EffectInstance effectInstance in m_EffectInstances)
        {
            if (effectInstance.Effect.EffectType == effectType)
            {
                return true;
            }
        }

        return false;
    }

    public void EndTurn()
    {
        for (int i = m_EffectInstances.Count - 1; i >= 0; --i)
        {
            EffectInstance effectInstance = m_EffectInstances[i];
            effectInstance.EndTurn(gameObject);

            if (!effectInstance.HasDuration)
            {
                continue;
            }

            effectInstance.RemainingTurns--;
            if (effectInstance.RemainingTurns == 0)
            {
                StopEffect(m_EffectInstances[i]);
                m_EffectInstances.RemoveAt(i);
            }
        }
    }

    private void StartEffect(EffectInstance effectInstance)
    {
        foreach (GameplayActionInstance instance in effectInstance.GameplayActionInstances)
        {
            instance.InitAction(gameObject);
            instance.StartAction(gameObject);
        }
    }

    private void StopEffect(EffectInstance effectInstance)
    {
        foreach (GameplayActionInstance instance in effectInstance.GameplayActionInstances)
        {
            instance.StopAction(gameObject);
        }
    }
}