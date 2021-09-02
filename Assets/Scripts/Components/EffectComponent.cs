using UnityEngine;
using System.Collections.Generic;

public class EffectInstance
{
    public Effect Effect { get; set; }
    public int RemainingTurns { get; set; }
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
        EffectInstance effectInstance = new EffectInstance { Effect = effect, RemainingTurns = nbTurns };
        effectInstance.Effect.StartEffect(gameObject);
        m_EffectInstances.Add(effectInstance);
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
            effectInstance.RemainingTurns--;
            if (effectInstance.RemainingTurns == 0)
            {
                effectInstance.Effect.StopEffect(gameObject);
                m_EffectInstances.RemoveAt(i);
            }
        }
    }
}