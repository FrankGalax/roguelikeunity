using System;
using TMPro;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int MaxHP;
    public int Armor;
    public GameObject Corpse;

    public int CurrentHP { get; private set; }
    public bool IsAlive { get; private set; }

    private void Awake()
    {
        IsAlive = true;
        CurrentHP = MaxHP;
    }

    public void TakeDamage(GameObject instigator, int damage)
    {
        if (!IsAlive)
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

            GameObject damagePopUp = Instantiate(Config.Instance.DamagePopup, 
                transform.position + Config.Instance.DamagePopupOffset, 
                Quaternion.identity);
            TextMeshPro textMeshPro = damagePopUp.GetComponent<TextMeshPro>();
            textMeshPro.text = damage.ToString();

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
        Debug.Log(gameObject.name + " Dies");
        IsAlive = false;
        Tile tile = GetComponent<Tile>();
        if (Corpse != null)
        {
            Instantiate(Corpse, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
        }
        FindObjectOfType<GameMap>().RemoveActor(tile);
    }
}
