using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    public List<Item> Items;
    public Signal UpdateInventorySignal { get; private set; }

    private void Awake()
    {
        Items = new List<Item>();
        UpdateInventorySignal = new Signal();
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
            foreach (ItemEffect itemEffect in item.ItemEffects)
            {
                itemEffect.Apply(gameObject);
            }
            RemoveItem(item);
        }
    }
}
