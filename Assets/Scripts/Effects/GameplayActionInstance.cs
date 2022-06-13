using UnityEngine;

public abstract class GameplayActionInstance
{
    public GameplayAction GameplayAction { get; set; }
    protected GameObject m_Instigator = null;

    public virtual void InitAction(GameObject gameObject, GameObject instigator) 
    {
        m_Instigator = instigator;
    }

    public virtual void StartAction(GameObject gameObject) { }
    public virtual void StopAction(GameObject gameObject) { }
    public virtual void EndTurn(GameObject gameObject, int turnCount) { }
}