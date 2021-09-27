using UnityEngine;
using System.Collections.Generic;

public enum SpellTargetType
{
    RandomEnemy,
    SingleTarget,
    AreaTarget,
    DirectionalTarget
}

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class Spell : ScriptableObject
{
    public SpellTargetType SpellTargetType = SpellTargetType.SingleTarget;
    public List<SpellEffect> SpellEffects;
    public int Radius;

    public SpellInstance CreateInstance()
    {
        SpellInstance instance = new SpellInstance
        {
            SpellTargetType = SpellTargetType,
            SpellEffects = SpellEffects,
            Radius = Radius
        };

        return instance;
    }
}
