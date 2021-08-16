using System;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int MaxHP;
    public int Armor;
    public GameObject Corpse;

    public int CurrentHP { get; private set; }
    private bool m_IsAlive;

    private void Awake()
    {
        m_IsAlive = true;
        CurrentHP = MaxHP;
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
            CurrentHP = Math.Max(CurrentHP - damage, 0);

            if (CurrentHP == 0)
            {
                Die();
            }
        }
    }

    public void Heal(int amount)
    {
        if (amount > MaxHP - CurrentHP)
        {
            amount = MaxHP - CurrentHP;
        }

        CurrentHP += amount;
    }

    private void Die()
    {
        Tile tile = GetComponent<Tile>();
        if (Corpse != null)
        {
            Instantiate(Corpse, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
        }
        Destroy(gameObject);
        FindObjectOfType<GameMap>().RemoveActor(tile);
    }
}
