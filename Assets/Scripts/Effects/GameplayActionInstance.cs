using UnityEngine;

public abstract class GameplayActionInstance
{
    public GameplayAction GameplayAction { get; set; }

    public virtual void InitAction(GameObject gameObject) { }
    public virtual void StartAction(GameObject gameObject) { }
    public virtual void StopAction(GameObject gameObject) { }
    public virtual void EndTurn(GameObject gameObject, int turnCount) { }
}