using System;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int MaxHP;
    public int Armor;
    public GameObject Corpse;

    private bool m_IsAlive;
    private int m_CurrentHP;

    private void Awake()
    {
        m_IsAlive = true;
        m_CurrentHP = MaxHP;
    }

    public void TakeDamage(GameObject instigator, int damage)
    {
        if (!m_IsAlive)
        {
            return;
        }

        damage -= Armor;

        if (damage < 0)
        {
            damage = 0;
        }

        if (damage > 0)
        {
            m_CurrentHP = Math.Max(m_CurrentHP - damage, 0);

            if (m_CurrentHP == 0)
            {
                Die();
            }
        }
    }

    private void Heal(int amount)
    {
        if (amount > MaxHP - m_CurrentHP)
        {
            amount = MaxHP - m_CurrentHP;
        }

        m_CurrentHP += amount;
    }

    private void Die()
    {
        if (Corpse != null)
        {
            Tile tile = GetComponent<Tile>();
            Instantiate(Corpse, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
