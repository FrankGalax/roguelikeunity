using UnityEngine;
using System.Collections.Generic;

public enum EffectType
{
    None,
    Confusion,
    Sleep
}

public class Effect
{
    public EffectType EffectType { get; set; }
    public int RemainingTurns { get; set; }
}

public class EffectComponent : MonoBehaviour
{
    private List<Effect> m_Effects;

    private void Awake()
    {
        m_Effects = new List<Effect>();
    }

    public void AddEffect(EffectType effectType, int nbTurns)
    {
        Effect effect = new Effect { EffectType = effectType, RemainingTurns = nbTurns };
        m_Effects.Add(effect);
    }

    public bool HasEffect(EffectType effectType)
    {
        foreach (Effect effect in m_Effects)
        {
            if (effect.EffectType == effectType)
            {
                return true;
            }
        }

        return false;
    }

    public void EndTurn()
    {
        for (int i = m_Effects.Count - 1; i >= 0; --i)
        {
            m_Effects[i].RemainingTurns--;
            if (m_Effects[i].RemainingTurns == 0)
            {
                m_Effects.RemoveAt(i);
            }
        }
    }
}