using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject HPPanel;
    public GameObject DiedPanel;
    public TextMeshProUGUI FloorText;
    public Sprite FullHeart;
    public Sprite EmptyHeart;

    private DamageComponent m_PlayerDamageComponent;
    private List<GameObject> m_InventorySlots;
    private List<GameObject> m_HPSlots;
    private GameObject m_Player;

    private void Awake()
    {
        m_InventorySlots = new List<GameObject>();
        if (InventoryPanel != null)
        {
            for (int i = 0; i < InventoryPanel.transform.childCount; ++i)
            {
                m_InventorySlots.Add(InventoryPanel.transform.GetChild(i).gameObject);
            }
        }

        m_HPSlots = new List<GameObject>();
        if (HPPanel != null)
        {
            for (int i = 0; i < HPPanel.transform.childCount; ++i)
            {
                m_HPSlots.Add(HPPanel.transform.GetChild(i).gameObject);
            }
        }
    }

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerDamageComponent = m_Player.GetComponent<DamageComponent>();

        UpdateInventory();
        UpdatePlayerHealth();
        UpdateFloorLevel();

        if (m_Player != null)
        {
            m_Player.GetComponent<InventoryComponent>().UpdateInventorySignal.AddSlot(() => UpdateInventory());
            DamageComponent damageComponent = m_Player.GetComponent<DamageComponent>();
            damageComponent.UpdateHealthSignal.AddSlot(() => UpdatePlayerHealth());
            damageComponent.DiedSignal.AddSlot(() => OnPlayerDied());
        }
    }

    private void UpdateFloorLevel()
    {
        FloorText.text = "Floor " + GameManager.Instance.CurrentFloor;
    }

    private void UpdateInventory()
    {
        InventoryComponent inventoryComponent = m_Player.GetComponent<InventoryComponent>();
        for (int i = 0; i < m_InventorySlots.Count; ++i)
        {
            GameObject inventorySlot = m_InventorySlots[i];
            Button button = inventorySlot.GetComponent<Button>();
            Image inventorySlotImage = inventorySlot.GetComponent<Image>();
            Image image = inventorySlot.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI text = inventorySlot.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Item item = inventoryComponent.Items.Count > i ? inventoryComponent.Items[i] : null;
            if (item != null)
            {
                inventorySlotImage.color = new Color(inventorySlotImage.color.r, inventorySlotImage.color.g, inventorySlotImage.color.b, 1.0f);
                button.interactable = true;
                image.gameObject.SetActive(true);
                text.gameObject.SetActive(true);
                image.sprite = item.Sprite;
                text.text = item.ItemName;
            }
            else
            {
                inventorySlotImage.color = new Color(inventorySlotImage.color.r, inventorySlotImage.color.g, inventorySlotImage.color.b, 0.0f);
                button.interactable = false;
                image.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
                image.sprite = null;
                text.text = null;
            }
        }
    }

    public void OnInventoryItemClick(int index)
    {
        if (m_Player != null)
        {
            InventoryComponent inventoryComponent = m_Player.GetComponent<InventoryComponent>();
            if (inventoryComponent != null)
            {
                inventoryComponent.UseItem(index);
            }
        }
    }

    private void UpdatePlayerHealth()
    {
        int health = m_PlayerDamageComponent.CurrentHP;
        for (int i = 0; i < m_HPSlots.Count; ++i)
        {
            Image image = m_HPSlots[i].GetComponent<Image>();
            if (i < health)
            {
                image.sprite = FullHeart;
            }
            else
            {
                image.sprite = EmptyHeart;
            }
        }
    }

    private void OnPlayerDied()
    {
        DiedPanel.SetActive(true);
    }
}
