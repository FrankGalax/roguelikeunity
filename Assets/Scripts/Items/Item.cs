using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite Sprite;
    public List<ItemEffect> ItemEffects;
    public bool IsCastingSpell;

    public void Apply(GameObject gameObject, GameMap gameMap)
    {
        foreach (ItemEffect itemEffect in ItemEffects)
        {
            itemEffect.Apply(gameObject, gameMap);
        }
    }
}
