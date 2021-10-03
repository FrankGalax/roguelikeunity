using UnityEngine;

public class InstantiateGameObjectActionInstance : GameplayActionInstance
{
    private GameObject m_GameObject;
    private InstantiateGameObjectAction m_InstantiateGameObjectAction;

    public override void InitAction(GameObject gameObject)
    {
        base.InitAction(gameObject);

        m_InstantiateGameObjectAction = (InstantiateGameObjectAction)GameplayAction;
    }

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        m_GameObject = GameObject.Instantiate(m_InstantiateGameObjectAction.Prefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        m_GameObject.transform.localPosition = m_InstantiateGameObjectAction.Offset;

        Tile tile = gameObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.UpdateVisibility();
        }
    }

    public override void StopAction(GameObject gameObject)
    {
        base.StopAction(gameObject);

        GameObject.Destroy(m_GameObject);
    }
}
