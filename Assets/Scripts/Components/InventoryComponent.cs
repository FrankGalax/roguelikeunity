using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryComponent : MonoBehaviour
{
    public List<Item> Items;
    public int MaxItems;
    public bool IsUsingItem { get; set; }

    public Signal UpdateInventorySignal { get; private set; }

    private ActionQueue m_ActionQueue;

    private void Awake()
    {
        Items = new List<Item>();
        UpdateInventorySignal = new Signal();
        m_ActionQueue = FindObjectOfType<ActionQueue>();
        IsUsingItem = false;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (UpdateInventorySignal != null)
        {
            UpdateInventorySignal.ClearSlots();
        }
        m_ActionQueue = FindObjectOfType<ActionQueue>();
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
        UpdateInventorySignal.SendSignal();
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        UpdateInventorySignal.SendSignal();
    }

    public void UseItem(int index)
    {
        if (Items.Count > index)
        {
            Item item = Items[index];
            if (item != null)
            {
                if (item.IsCastingSpell)
                {
                    StartUsingItem();
                }
                else
                {
                    RemoveItem(item);
                }

                m_ActionQueue.AddAction(new UseItemAction { GameObject = gameObject, Item = item });
            }
        }
    }

    private void StartUsingItem()
    {
        IsUsingItem = true;
        UpdateInventorySignal.SendSignal();
    }

    public void StopUsingItem(Item item, bool isCanceled)
    {
        IsUsingItem = false;

        if (isCanceled)
        {
            UpdateInventorySignal.SendSignal();
        }
        else
        {
            RemoveItem(item);
        }
    }
}
