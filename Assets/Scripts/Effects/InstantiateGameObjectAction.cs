using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantiateGameObjectAction", menuName = "Effects/InstantiateGameObjectAction")]
public class InstantiateGameObjectAction : GameplayAction
{
    public GameObject Prefab;
    public Vector3 Offset;

    public override GameplayActionInstance CreateInstance()
    {
        return new InstantiateGameObjectActionInstance();
    }
}