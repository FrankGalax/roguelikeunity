using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : GameSingleton<Config>
{
    public GameObject DamagePopup;
    public Vector3 DamagePopupOffset;
    [EnumNamedArray(typeof(ItemEnum))]
    public Sprite[] ItemSprites = new Sprite[(int)ItemEnum.Count];
}
