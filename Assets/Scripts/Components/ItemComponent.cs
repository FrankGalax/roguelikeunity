using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public ItemEnum ItemEnum;
    public int Int1;

    public Item GetItem()
    {
        Item item = new Item();

        switch (ItemEnum)
        {
            case ItemEnum.HealthPotion:
                item.AddItemEffect(new HealItemEffect(Int1));
                break;
        }

        return item;
    }
}
