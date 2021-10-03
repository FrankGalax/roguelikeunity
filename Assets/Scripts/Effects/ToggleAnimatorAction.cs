using UnityEngine;

[CreateAssetMenu(fileName = "NewToggleAnimatorAction", menuName = "Effects/ToggleAnimatorAction")]
public class ToggleAnimatorAction : GameplayAction
{
    public int TurnsEnabled = 1;
    public int TurnsDisabled = 1;
    public bool StartEnabled = true;

    public override GameplayActionInstance CreateInstance()
    {
        return new ToggleAnimatorActionInstance();
    }
}