using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DamageType
{
    Physical,
    Cold,
    Lightning,
    Fire
}

public class DamageComponent : MonoBehaviour
{
    public int MaxHP;
    public int Armor;
    public GameObject Corpse;

    public int CurrentHP { get; private set; }
    public bool IsAlive { get; private set; }
    public Signal UpdateHealthSignal { get; private set; }
    public Signal DiedSignal { get; private set; }
    public bool IsInvulnerable { get; set; }

    private void Awake()
    {
        IsAlive = true;
        CurrentHP = MaxHP;
        UpdateHealthSignal = new Signal();
        DiedSignal = new Signal();
        IsInvulnerable = false;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (UpdateHealthSignal != null)
        {
            UpdateHealthSignal.ClearSlots();
        }

        if (DiedSignal != null)
        {
            DiedSignal.ClearSlots();
        }
    }

    public void TakeDamage(GameObject instigator, int damage, DamageType damageType)
    {
        if (!IsAlive || IsInvulnerable)
        {
            return;
        }

        if (damageType == DamageType.Physical)
        {
            damage -= Armor;
        }

        if (damage < 0)
        {
            damage = 0;
        }

        if (damage > 0)
        {
            CurrentHP = Math.Max(CurrentHP - damage, 0);

            UpdateHealthSignal.SendSignal();

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

        if (amount == 0)
        {
            return;
        }

        CurrentHP += amount;

        UpdateHealthSignal.SendSignal();

        GameObject healPopUp = Instantiate(Config.Instance.HealPopup,
                transform.position + Config.Instance.DamagePopupOffset,
                Quaternion.identity);
        TextMeshPro textMeshPro = healPopUp.GetComponent<TextMeshPro>();
        textMeshPro.text = amount.ToString();
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

        DiedSignal.SendSignal();

        FindObjectOfType<GameMap>().RemoveActor(tile);
    }
}
