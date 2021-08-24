using UnityEngine;

public abstract class AreaControl : MonoBehaviour
{
    public int NbTurns;

    private int m_RemainingTurns;

    private void Awake()
    {
        m_RemainingTurns = NbTurns;
    }

    public virtual void OnEnter(GameObject gameObject) { }
    
    public void OnEndTurn(GameMap gameMap)
    {
        m_RemainingTurns--;
        if (m_RemainingTurns == 0)
        {
            gameMap.RemoveAreaControl(this);
            Destroy(gameObject);
        }
    }
}