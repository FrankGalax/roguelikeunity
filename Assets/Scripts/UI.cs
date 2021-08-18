using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public GameObject Player;

    private TextMeshProUGUI m_PlayerHPText;
    private DamageComponent m_PlayerDamageComponent;

    private void Awake()
    {
        m_PlayerHPText = transform.Find("PlayerHP").GetComponent<TextMeshProUGUI>();
        m_PlayerDamageComponent = Player.GetComponent<DamageComponent>();
    }

    private void Update()
    {
        m_PlayerHPText.text = m_PlayerDamageComponent.CurrentHP + "/" + m_PlayerDamageComponent.MaxHP;
    }
}
