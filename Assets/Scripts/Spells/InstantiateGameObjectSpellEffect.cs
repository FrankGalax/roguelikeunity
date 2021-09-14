using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantiateGameObjectSpellEffect", menuName = "Spells/InstantiateGameObjectSpellEffect")]
public class InstantiateGameObjectSpellEffect : SpellEffect
{
    public GameObject GameObject;

    public override void TargetTile(GameObject gameObject, Tile tile)
    {
        base.TargetTile(gameObject, tile);

        Instantiate(GameObject, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
    }
}