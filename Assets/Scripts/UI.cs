using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject Player;

    private Text m_PlayerHPText;
    private DamageComponent m_PlayerDamageComponent;

    private void Awake()
    {
        m_PlayerHPText = transform.Find("PlayerHP").GetComponent<Text>();
        m_PlayerDamageComponent = Player.GetComponent<DamageComponent>();
    }

    private void Update()
    {
        m_PlayerHPText.text = m_PlayerDamageComponent.CurrentHP + "/" + m_PlayerDamageComponent.MaxHP;
    }
}
