using System;
using System.Collections.Generic;
using UnityEngine;

public class Config : GameSingleton<Config>
{
    public GameObject DamagePopup;
    public GameObject HealPopup;
    public Vector3 DamagePopupOffset;
    public float MoveSpeed = 4.0f;
    public GameObject SingleTarget;
    public GameObject AreaSelection;
    public GameObject AreaSelectionTop;
    public GameObject AreaSelectionTopLeft;
    public GameObject AreaSelectionLeft;
    public GameObject AreaSelectionBottomLeft;
    public GameObject AreaSelectionBottom;
    public GameObject AreaSelectionBottomRight;
    public GameObject AreaSelectionRight;
    public GameObject AreaSelectionTopRight;
    public GameObject DirectionalSelection;
    public GameObject DiagonalDirectionalSelection;
    public GameObject DebugTile;
    public List<FloorDefinition> FloorDefinitions;
}
