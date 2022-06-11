using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardUI
{
    public Image Image { get; set; }
    public TextMeshProUGUI Text { get; set; }
    public Button Button { get; set; }
}

public class UI : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject HPPanel;
    public RectTransform WhiteMana;
    public RectTransform BlueMana;
    public GameObject DiedPanel;
    public GameObject SpellPanel;
    public TextMeshProUGUI FloorText;
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public GameObject RewardPanel;
    public List<GameObject> RewardPanels;

    private DamageComponent m_PlayerDamageComponent;
    private InventoryComponent m_PlayerInventoryComponent;
    private SpellComponent m_PlayerSpellComponent;
    private List<GameObject> m_InventorySlots;
    private List<GameObject> m_HPSlots;
    private GameObject m_Player;
    private List<RewardUI> m_RewardUIs;
    private float m_CurrentManaWidth;
    private float m_TargetManaWidth;

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

        m_RewardUIs = new List<RewardUI>();
        foreach (GameObject rewardPanel in RewardPanels)
        {
            RewardUI reward = new RewardUI();
            reward.Image = rewardPanel.transform.GetChild(0).GetComponent<Image>();
            reward.Text = rewardPanel.transform.GetComponentInChildren<TextMeshProUGUI>();
            reward.Button = rewardPanel.GetComponent<Button>();
            m_RewardUIs.Add(reward);
        }
    }

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerDamageComponent = m_Player.GetComponent<DamageComponent>();
        m_PlayerInventoryComponent = m_Player.GetComponent<InventoryComponent>();
        m_PlayerSpellComponent = m_Player.GetComponent<SpellComponent>();

        UpdateInventory();
        UpdatePlayerHealth();
        UpdateFloorLevel();
        UpdateSpells();
        InitMana();

        if (m_Player != null)
        {
            m_PlayerInventoryComponent.UpdateInventorySignal.AddSlot(UpdateInventory);
            m_PlayerDamageComponent.UpdateHealthSignal.AddSlot(UpdatePlayerHealth);
            m_PlayerDamageComponent.DiedSignal.AddSlot(OnPlayerDied);
            m_PlayerSpellComponent.SpellsUpdatedSignal.AddSlot(UpdateSpells);
            m_PlayerSpellComponent.ManaUpdatedSignal.AddSlot(UpdateMana);
        }
    }

    private void Update()
    {
        LerpMana();
    }

    public void OnInventoryItemClick(int index)
    {
        if (m_Player != null)
        {
            if (m_PlayerInventoryComponent != null)
            {
                m_PlayerInventoryComponent.UseItem(index);
            }
        }
    }

    public void ShowRewards()
    {
        List<Reward> floorRewards = GameManager.Instance.FloorRewards;

        RewardPanel.SetActive(true);

        for (int i = 0; i < m_RewardUIs.Count; ++i)
        {
            RewardUI rewardUI = m_RewardUIs[i];
            if (i < floorRewards.Count)
            {
                rewardUI.Image.enabled = true;
                rewardUI.Image.sprite = floorRewards[i].Icon;
                rewardUI.Text.enabled = true;
                rewardUI.Text.text = floorRewards[i].Name;
                rewardUI.Button.interactable = true;
            }
            else
            {
                rewardUI.Image.enabled = false;
                rewardUI.Text.enabled = false;
                rewardUI.Button.interactable = false;
            }
        }
    }

    public void SelectReward(int rewardIndex)
    {
        GameManager.Instance.SelectReward(rewardIndex);
    }

    private void UpdateFloorLevel()
    {
        FloorText.text = "Floor " + GameManager.Instance.CurrentFloor;
    }

    private void UpdateInventory()
    {
        for (int i = 0; i < m_InventorySlots.Count; ++i)
        {
            GameObject inventorySlot = m_InventorySlots[i];
            Button button = inventorySlot.GetComponent<Button>();
            Image inventorySlotImage = inventorySlot.GetComponent<Image>();
            Image image = inventorySlot.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI text = inventorySlot.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Item item = m_PlayerInventoryComponent.Items.Count > i ? m_PlayerInventoryComponent.Items[i] : null;
            if (item != null)
            {
                inventorySlotImage.color = new Color(inventorySlotImage.color.r, inventorySlotImage.color.g, inventorySlotImage.color.b, 1.0f);
                button.interactable = !m_PlayerInventoryComponent.IsUsingItem;
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

    private void UpdateSpells()
    {
        for (int i = 0; i < SpellPanel.transform.childCount; ++i)
        {
            GameObject spellIcon = SpellPanel.transform.GetChild(i).gameObject;
            SpellInstance spellInstance = m_PlayerSpellComponent.GetSpellInstance(i);

            spellIcon.SetActive(spellInstance != null);
            if (spellInstance != null)
            {
                spellIcon.GetComponent<Image>().sprite = spellInstance.Spell.Icon;
            }
        }
    }

    private void InitMana()
    {
        float manaRatio = m_PlayerSpellComponent.GetManaRatio();
        m_CurrentManaWidth = WhiteMana.rect.width * manaRatio;
        m_TargetManaWidth = m_CurrentManaWidth;
    }

    private void UpdateMana()
    {
        float manaRatio = m_PlayerSpellComponent.GetManaRatio();
        m_TargetManaWidth = WhiteMana.rect.width * manaRatio;
    }

    private void LerpMana()
    {
        m_CurrentManaWidth = Mathf.Lerp(m_CurrentManaWidth, m_TargetManaWidth, 6.0f * Time.deltaTime);
        BlueMana.sizeDelta = new Vector2(m_CurrentManaWidth, BlueMana.rect.height);
    }
}
