using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class SpellComponent : MonoBehaviour
{
    public int SpellCount = 4;
    public int MaxMana = 10;

    private List<SpellInstance> m_SpellInstances;
    private int m_CurrentMana = 0;

    public Signal SpellsUpdatedSignal { get; private set; }
    public Signal ManaUpdatedSignal { get; private set; }

    private void Awake()
    {
        m_SpellInstances = new List<SpellInstance>();
        
        for (int i = 0; i < SpellCount; ++i)
        {
            m_SpellInstances.Add(null);
        }

        m_CurrentMana = MaxMana;

        SpellsUpdatedSignal = new Signal();
        ManaUpdatedSignal = new Signal();
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

        if (ManaUpdatedSignal != null)
        {
            ManaUpdatedSignal.ClearSlots();
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

    public bool CanCastSpell(int index)
    {
        if (index < 0 || index >= SpellCount)
        {
            return false;
        }

        return m_SpellInstances[index].ManaCost <= m_CurrentMana;
    }

    public SpellInstance GetSpellInstance(int index)
    {
        if (index < 0 || index >= SpellCount)
        {
            return null;
        }

        return m_SpellInstances[index];
    }

    public void Cast(int index, GameObject gameObject, GameMap gameMap)
    {
        Assert.IsTrue(CanCastSpell(index));
        m_SpellInstances[index].Cast(gameObject, gameMap);
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

    public void ConsumeMana(int mana)
    {
        m_CurrentMana -= mana;
        Assert.IsTrue(m_CurrentMana >= 0);
        if (m_CurrentMana < 0)
        {
            m_CurrentMana = 0;
        }

        ManaUpdatedSignal.SendSignal();
    }

    public float GetManaRatio()
    {
        return m_CurrentMana / (float)MaxMana;
    }
}