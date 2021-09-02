using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantiateGameObjectGameplayAction", menuName = "Effects/InstantiateGameObjectGameplayAction")]
public class InstantiateGameObjectGameplayAction : GameplayAction
{
    public GameObject Prefab;
    public Vector3 Offset;

    private GameObject m_GameObject;

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        m_GameObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        m_GameObject.transform.localPosition = Offset;
    }

    public override void StopAction(GameObject gameObject)
    {
        base.StopAction(gameObject);

        GameObject.Destroy(m_GameObject);
    }
}