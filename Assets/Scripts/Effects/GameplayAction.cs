using UnityEngine;

public abstract class GameplayAction : ScriptableObject
{
    public virtual void StartAction(GameObject gameObject) { }
    public virtual void StopAction(GameObject gameObject) { }
}