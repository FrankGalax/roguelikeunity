using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantiateGameObjectGameplayAction", menuName = "Effects/InstantiateGameObjectGameplayAction")]
public class InstantiateGameObjectGameplayAction : GameplayAction
{
    public GameObject Prefab;
    public Vector3 Offset;

    public override GameplayActionInstance CreateInstance()
    {
        return new InstantiateGameObjectGameplayActionInstance();
    }
}