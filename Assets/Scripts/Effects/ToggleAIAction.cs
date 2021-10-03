using UnityEngine;

[CreateAssetMenu(fileName = "NewToggleAIAction", menuName = "Effects/ToggleAIAction")]
public class ToggleAIAction : GameplayAction
{
    public int TurnsEnabled = 1;
    public int TurnsDisabled = 1;
    public bool StartEnabled = true;

    public override GameplayActionInstance CreateInstance()
    {
        return new ToggleAIActionInstance();
    }
}