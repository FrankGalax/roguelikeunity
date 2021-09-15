using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaControlSpellEffect", menuName = "Spells/AreaControlSpellEffect")]
public class AreaControlSpellEffect : SpellEffect
{
    public GameObject AreaControl;

    public override void TargetTile(GameObject gameObject, Tile tile, int radius)
    {
        base.TargetTile(gameObject, tile, radius);

        GameObject areaControlGameObject = Instantiate(AreaControl, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
        AreaControl areaControl = areaControlGameObject.GetComponent<AreaControl>();
        FindObjectOfType<GameMap>().AddAreaControl(areaControl);
    }
}