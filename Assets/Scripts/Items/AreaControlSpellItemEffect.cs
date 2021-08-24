using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAreaControlSpellItemEffect", menuName = "Items/AreaControlSpellItemEffect")]
public class AreaControlSpellItemEffect : ItemEffect
{
    public GameObject AreaControl;
    public int Radius;

    public override void Apply(GameObject gameObject, GameMap gameMap)
    {
        gameMap.GetComponent<ActionQueue>().AddAction(new UseAreaTargetSpellAction { GameObject = gameObject, Radius = Radius, SpellCallback = UseSpell });
    }

    private bool UseSpell(GameMap gameMap, Tile target)
    {
        List<Tile> floors = gameMap.GetFloorsInRange(target.X, target.Y, Radius);

        if (floors.Count == 0)
        {
            return false;
        }

        foreach (Tile floor in floors)
        {
            GameObject gameObject = Instantiate(AreaControl, new Vector3((float)floor.X, (float)floor.Y, 0.0f), Quaternion.identity);
            AreaControl areaControl = gameObject.GetComponent<AreaControl>();
            gameMap.AddAreaControl(areaControl);
        }

        return true;
    }
}