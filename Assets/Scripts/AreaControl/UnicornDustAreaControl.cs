using UnityEngine;

public class UnicornDustAreaControl : AreaControl
{
    public int SleepNbTurns;

    public override void OnEnter(GameObject gameObject)
    {
        base.OnEnter(gameObject);

        AIComponent aiComponent = gameObject.GetComponent<AIComponent>();
        if (aiComponent != null)
        {
            aiComponent.Sleep(SleepNbTurns);
        }
    }
}