using UnityEngine;

public class InstantiateGameObjectGameplayActionInstance : GameplayActionInstance
{
    private GameObject m_GameObject;
    private InstantiateGameObjectGameplayAction m_InstantiateGameObjectGameplayAction;

    public override void InitAction(GameObject gameObject)
    {
        base.InitAction(gameObject);

        m_InstantiateGameObjectGameplayAction = (InstantiateGameObjectGameplayAction)GameplayAction;
    }

    public override void StartAction(GameObject gameObject)
    {
        base.StartAction(gameObject);

        m_GameObject = GameObject.Instantiate(m_InstantiateGameObjectGameplayAction.Prefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        m_GameObject.transform.localPosition = m_InstantiateGameObjectGameplayAction.Offset;

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
