using UnityEngine;

public class ColorActionInstance : GameplayActionInstance
{
    private int m_ColorModifierId;
    private ColorAction m_ColorAction;

    public override void InitAction(GameObject gameObject)
    {
        base.InitAction(gameObject);

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
            m_ColorModifierId = colorComponent.AddColorModifier(m_ColorAction.Color);
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
            colorComponent.RemoveColorModifier(m_ColorModifierId);
        }
    }
}