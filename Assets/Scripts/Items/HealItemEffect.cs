using UnityEngine;

[CreateAssetMenu(fileName = "NewHealItemEffect", menuName = "Items/HealItemEffect")]
public class HealItemEffect : ItemEffect
{
    public int Amount;

    public override void Apply(GameObject gameObject, GameMap gameMap, Item item)
    {
        DamageComponent damageComponent = gameObject.GetComponent<DamageComponent>();
        if (damageComponent != null)
        {
            damageComponent.Heal(Amount);
        }
    }
}
