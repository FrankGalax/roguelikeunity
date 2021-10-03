using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpellComponent : MonoBehaviour
{
    public int SpellCount = 4;

    private List<SpellInstance> m_SpellInstances;
    public Signal SpellsUpdatedSignal { get; private set; }

    private void Awake()
    {
        m_SpellInstances = new List<SpellInstance>();
        
        for (int i = 0; i < SpellCount; ++i)
        {
            m_SpellInstances.Add(null);
        }

        SpellsUpdatedSignal = new Signal();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SpellsUpdatedSignal != null)
        {
            SpellsUpdatedSignal.ClearSlots();
        }
    }

    public void LearnSpell(Spell spell)
    {
        for (int i = 0; i < SpellCount; ++i)
        {
            if (m_SpellInstances[i] == null)
            {
                m_SpellInstances[i] = spell.CreateInstance();
                SpellsUpdatedSignal.SendSignal();
                return;
            }
        }
    }

    public SpellInstance GetSpellInstance(int index)
    {
        if (index < 0 || index >= SpellCount)
        {
            return null;
        }

        return m_SpellInstances[index];
    }

    public bool KnowsSpell(Spell spell)
    {
        foreach (SpellInstance spellInstance in m_SpellInstances)
        {
            if (spellInstance != null && spellInstance.Spell == spell)
            {
                return true;
            }
        }

        return false;
    }
}