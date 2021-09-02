using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Effects/Effect")]
public class Effect : ScriptableObject
{
    public EffectType EffectType;
    public List<GameplayAction> GameplayActions;

    public void StartEffect(GameObject gameObject)
    {
        foreach (GameplayAction gameplayAction in GameplayActions)
        {
            gameplayAction.StartAction(gameObject);
        }
    }
    public void StopEffect(GameObject gameObject)
    {
        foreach (GameplayAction gameplayAction in GameplayActions)
        {
            gameplayAction.StopAction(gameObject);
        }
    }
}