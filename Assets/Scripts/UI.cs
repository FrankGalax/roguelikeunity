using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public GameObject Player;
    public GameObject InventoryPanel;

    private TextMeshProUGUI m_PlayerHPText;
    private DamageComponent m_PlayerDamageComponent;
    private List<GameObject> m_InventorySlots;

    private void Awake()
    {
        m_PlayerHPText = transform.Find("PlayerHP").GetComponent<TextMeshProUGUI>();
        m_PlayerDamageComponent = Player.GetComponent<DamageComponent>();
        m_InventorySlots = new List<GameObject>();
        if (InventoryPanel != null)
        {
            for (int i = 0; i < InventoryPanel.transform.childCount; ++i)
            {
                m_InventorySlots.Add(InventoryPanel.transform.GetChild(i).gameObject);
            }
        }
    }

    private void Start()
    {
        UpdateInventory();

        if (Player != null)
        {
            Player.GetComponent<InventoryComponent>().UpdateInventorySignal.AddSlot(() => UpdateInventory());
        }
    }

    private void Update()
    {
        m_PlayerHPText.text = m_PlayerDamageComponent.CurrentHP + "/" + m_PlayerDamageComponent.MaxHP;
    }

    public void UpdateInventory()
    {
        InventoryComponent inventoryComponent = Player.GetComponent<InventoryComponent>();
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
        if (Player != null)
        {
            InventoryComponent inventoryComponent = Player.GetComponent<InventoryComponent>();
            if (inventoryComponent != null)
            {
                inventoryComponent.UseItem(index);
            }
        }
    }
}
