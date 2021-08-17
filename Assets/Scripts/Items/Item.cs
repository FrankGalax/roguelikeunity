using System.Collections.Generic;

public class Item
{
    private List<ItemEffect> m_ItemEffects;

    public Item()
    {
        m_ItemEffects = new List<ItemEffect>();
    }

    public void AddItemEffect(ItemEffect itemEffect)
    {
        m_ItemEffects.Add(itemEffect);
    }
}
