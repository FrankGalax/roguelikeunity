using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageAction", menuName = "Effects/DamageAction")]
public class DamageAction : GameplayAction
{
    public int Damage;
    public DamageType DamageType;

    public override GameplayActionInstance CreateInstance()
    {
        return new DamageActionInstance();
    }
}
