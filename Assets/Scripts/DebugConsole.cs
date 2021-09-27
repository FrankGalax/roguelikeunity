using TMPro;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract class DebugCommandBase
{
    public string Name { get; set; }
    public abstract void Invoke(List<string> commandParams);
}

public class DebugCommand : DebugCommandBase
{
    public Action Action { get; set; }

    public override void Invoke(List<string> commandParams)
    {
        Action();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    public Action<T> Action { get; set; }

    public override void Invoke(List<string> commandParams)
    {
        T commandParam = (T)Convert.ChangeType(commandParams[0], typeof(T));
        Action(commandParam);
    }
}

public class DebugConsole : MonoBehaviour
{
    public GameObject DebugPanel;
    public TextMeshProUGUI SuggestionsText;
    public TMP_InputField InputField;
    public int MaxSuggestions = 5;

    private List<DebugCommandBase> m_DebugCommands;
    private List<DebugCommandBase> m_Suggestions;

    private void Awake()
    {
        m_DebugCommands = new List<DebugCommandBase>();
        m_Suggestions = new List<DebugCommandBase>();

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
        m_DebugCommands.Add(new DebugCommand<string>
        {
            Name = "spawn",
            Action = (name) =>
            {
                GameMap gameMap = FindObjectOfType<GameMap>();
                gameMap.CheatSpawn(name);
            }
        });
        m_DebugCommands.Add(new DebugCommand
        {
            Name = "changefloor",
            Action = () =>
            {
                ActionQueue actionQueue = FindObjectOfType<ActionQueue>();
                actionQueue.CheatChangeFloor();
            }
        });
        m_DebugCommands.Add(new DebugCommand
        {
            Name = "killinvisibles",
            Action = () =>
            {
                GameMap gameMap = FindObjectOfType<GameMap>();
                gameMap.CheatKillInvisibles();
            }
        });
        m_DebugCommands.Add(new DebugCommand
        {
            Name = "suicide",
            Action = () =>
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    DamageComponent damageComponent = player.GetComponent<DamageComponent>();
                    damageComponent.TakeDamage(player, 1000, DamageType.Physical);
                }
            }
        });
        m_DebugCommands.Add(new DebugCommand<int>
        {
            Name = "setfloor",
            Action = (floorIndex) =>
            {
                ActionQueue actionQueue = FindObjectOfType<ActionQueue>();
                actionQueue.CheatSetFloor(floorIndex);
            }
        });
        m_DebugCommands.Add(new DebugCommand<string>
        {
            Name = "learnspell",
            Action = (spellName) =>
            {
                foreach (Spell spell in Config.Instance.Spells)
                {
                    if (spell.name.ToLower().Contains(spellName.ToLower()))
                    {
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        SpellComponent spellComponent = player.GetComponent<SpellComponent>();
                        spellComponent.LearnSpell(spell);
                        return;
                    }
                }
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
                if (InvokeCommand())
                {
                    DebugPanel.SetActive(false);
                    GameManager.Instance.IsPaused = false;
                }
            }
        }
    }

    public void UpdateSuggestions()
    {
        string[] values = InputField.text.Split(' ');
        string value = values.Length > 0 ? values[0] : "";

        m_Suggestions.Clear();
        foreach (DebugCommandBase debugCommand in m_DebugCommands)
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

    private bool InvokeCommand()
    {
        foreach (DebugCommandBase debugCommandBase in m_DebugCommands)
        {
            string[] text = InputField.text.Split(' ');
            string commandName = text[0];
            List<string> commandParams = text.Length > 1 ? text.Skip(1).ToList() : new List<string>();
            if (debugCommandBase.Name == commandName)
            {
                debugCommandBase.Invoke(commandParams);
                return true;
            }
        }

        return false;
    }
}