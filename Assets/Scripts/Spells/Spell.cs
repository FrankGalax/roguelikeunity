using UnityEngine;
using System.Collections.Generic;

public enum SpellTargetType
{
    RandomEnemy,
    SingleTarget,
    AreaTarget,
    DirectionalTarget,
    Self
}

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public SpellTargetType SpellTargetType = SpellTargetType.SingleTarget;
    public List<SpellEffect> SpellEffects;
    public int Radius;
    public Sprite Icon;
    public string Name;
    public int ManaCost;

    public SpellInstance CreateInstance()
    {
        SpellInstance instance = new SpellInstance
        {
            SpellTargetType = SpellTargetType,
            SpellEffects = SpellEffects,
            Radius = Radius,
            ManaCost = ManaCost,
            Spell = this
        };

        return instance;
    }
}
