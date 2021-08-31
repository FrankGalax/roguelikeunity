using TMPro;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class DebugCommand
{
    public string Name { get; set; }
    public Action Action { get; set; }
}

public class DebugConsole : MonoBehaviour
{
    public GameObject DebugPanel;
    public TextMeshProUGUI SuggestionsText;
    public TMP_InputField InputField;
    public int MaxSuggestions = 5;

    private List<DebugCommand> m_DebugCommands;
    private List<DebugCommand> m_Suggestions;

    private void Awake()
    {
        m_DebugCommands = new List<DebugCommand>();
        m_Suggestions = new List<DebugCommand>();

        m_DebugCommands.Add(new DebugCommand
        {
            Name = "showtiles",
            Action = () =>
            {
                GameMap gameMap = FindObjectOfType<GameMap>();
                gameMap.CheatShowAllTiles();
            }
        });
        m_DebugCommands.Add(new DebugCommand
        {
            Name = "healplayer",
            Action = () =>
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                DamageComponent damageComponent = player.GetComponent<DamageComponent>();
                damageComponent.Heal(damageComponent.MaxHP);
            }
        });
        m_DebugCommands.Sort((a, b) => a.Name.CompareTo(b.Name));
    }

    private void Start()
    {
        UpdateSuggestions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            bool isDebugActive = DebugPanel.activeSelf;
            isDebugActive = !isDebugActive;
            DebugPanel.SetActive(isDebugActive);
            GameManager.Instance.IsPaused = isDebugActive;

            if (isDebugActive)
            {
                InputField.text = "";
                UpdateSuggestions();
                InputField.Select();
                InputField.ActivateInputField();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isDebugActive = DebugPanel.activeSelf;
            if (isDebugActive)
            {
                if (m_Suggestions.Count > 0)
                {
                    InputField.text = m_Suggestions[0].Name;
                    InputField.caretPosition = InputField.text.Length;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            bool isDebugActive = DebugPanel.activeSelf;
            if (isDebugActive)
            {
                if (ApplyCommand())
                {
                    DebugPanel.SetActive(false);
                    GameManager.Instance.IsPaused = false;
                }
            }
        }
    }

    public void UpdateSuggestions()
    {
        string value = InputField.text;

        m_Suggestions.Clear();
        foreach (DebugCommand debugCommand in m_DebugCommands)
        {
            if (string.IsNullOrWhiteSpace(value) || debugCommand.Name.Contains(value))
            {
                m_Suggestions.Add(debugCommand);
            }

            if (m_Suggestions.Count == MaxSuggestions)
            {
                break;
            }
        }

        if (m_Suggestions.Count == 0)
        {
            SuggestionsText.text = "";
        }
        else
        {
            SuggestionsText.text = m_Suggestions.Select(p => p.Name).Aggregate((i, j) => i + "\n" + j);
        }
    }

    private bool ApplyCommand()
    {
        foreach (DebugCommand debugCommand in m_DebugCommands)
        {
            if (debugCommand.Name == InputField.text)
            {
                debugCommand.Action();
                return true;
            }
        }

        return false;
    }
}