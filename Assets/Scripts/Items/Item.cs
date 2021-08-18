using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite Sprite;
    public List<ItemEffect> ItemEffects;
}
