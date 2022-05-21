using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorModifier
{
    public int Id { get; set; }
    public Color Color { get; set; }
}

public class ColorComponent : MonoBehaviour
{
    public bool IsLerping;

    public Color InitialColor { get; private set; }

    private SpriteRenderer m_SpriteRenderer;
    private int m_NextColorModifierId;
    private List<ColorModifier> m_ColorModifiers;
    private Color m_BaseColor;
    private Color m_ReplaceColor;
    private bool m_HasColorReplacement;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_BaseColor = m_SpriteRenderer.color;
        InitialColor = m_SpriteRenderer.color;
        m_NextColorModifierId = 0;
        m_ColorModifiers = new List<ColorModifier>();
        m_HasColorReplacement = false;
    }

    private void Update()
    {
        if (!IsLerping)
        {
            return;
        }

        m_SpriteRenderer.color = Color.Lerp(m_SpriteRenderer.color, GetTargetColor(), Time.deltaTime * 6.0f);   
    }

    public void SetBaseColor(Color color)
    {
        m_BaseColor = color;
        ApplyColor();
    }

    public void AddReplaceColor(Color color)
    {
        m_ReplaceColor = color;
        m_HasColorReplacement = true;
        ApplyColor();
    }

    public void RemoveReplaceColor()
    {
        m_HasColorReplacement = false;
        ApplyColor();
    }

    public int AddColorModifier(Color color)
    {
        int colorModifierId = m_NextColorModifierId;
        m_NextColorModifierId++;

        m_ColorModifiers.Add(new ColorModifier { Id = colorModifierId, Color = color });
        ApplyColor();
        return colorModifierId;
    }

    public void RemoveColorModifier(int id)
    {
        for (int i = 0; i < m_ColorModifiers.Count; ++i)
        {
            if (m_ColorModifiers[i].Id == id)
            {
                m_ColorModifiers.RemoveAt(i);
                ApplyColor();
                return;
            }
        }
    }

    private Color GetTargetColor()
    {
        Color color = Color.white;
        if (m_HasColorReplacement)
        {
            color = new Color(m_ReplaceColor.r, m_ReplaceColor.g, m_ReplaceColor.b, m_ReplaceColor.a);
        }
        else
        {
            color = new Color(m_BaseColor.r, m_BaseColor.g, m_BaseColor.b, m_BaseColor.a);
        }

        foreach (ColorModifier colorModifier in m_ColorModifiers)
        {
            color *= colorModifier.Color;
        }

        return color;
    }

    private void ApplyColor()
    {
        if (IsLerping)
        {
            return;
        }

        m_SpriteRenderer.color = GetTargetColor();
    }
}
