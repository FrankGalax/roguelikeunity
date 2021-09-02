
using UnityEngine;

public class DestroyAfterTimer : MonoBehaviour
{
    public float DestroyTime;

    private float m_Timer;

    void Start()
    {
        m_Timer = DestroyTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
