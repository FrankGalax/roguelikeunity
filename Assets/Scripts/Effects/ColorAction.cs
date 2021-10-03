using UnityEngine;

[CreateAssetMenu(fileName = "NewColorAction", menuName = "Effects/ColorAction")]
public class ColorAction : GameplayAction
{
    public Color Color;

    public override GameplayActionInstance CreateInstance()
    {
        return new ColorActionInstance();
    }
}
