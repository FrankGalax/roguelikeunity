using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/Effect")]
public class Effect : ScriptableObject
{
    public EffectType EffectType;
    public List<GameplayAction> GameplayActions;
}