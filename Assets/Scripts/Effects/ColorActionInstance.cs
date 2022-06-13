using UnityEngine;

public class ColorActionInstance : GameplayActionInstance
{
    private ColorAction m_ColorAction;

    public override void InitAction(GameObject gameObject, GameObject instigator)
    {
        base.InitAction(gameObject, instigator);

        m_ColorAction = (ColorAction)GameplayAction;
    }

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        ColorComponent colorComponent = gameObject.GetComponent<ColorComponent>();
        if (colorComponent == null)
        {
            colorComponent = gameObject.GetComponentInChildren<ColorComponent>();
        }

        if (colorComponent != null)
        {
            colorComponent.AddReplaceColor(m_ColorAction.Color);
        }
    }

    public override void StopAction(GameObject gameObject)
    {
        base.StopAction(gameObject);

        ColorComponent colorComponent = gameObject.GetComponent<ColorComponent>();
        if (colorComponent == null)
        {
            colorComponent = gameObject.GetComponentInChildren<ColorComponent>();
        }

        if (colorComponent != null)
        {
            colorComponent.RemoveReplaceColor();
        }
    }
}