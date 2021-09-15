using UnityEngine;

[CreateAssetMenu(fileName = "NewInstantiateBeamSpellEffect", menuName = "Spells/InstantiateBeamSpellEffect")]
public class InstantiateBeamSpellEffect : SpellEffect
{
    public GameObject Beam;

    public override void TargetTile(GameObject gameObject, Tile tile, int radius)
    {
        base.TargetTile(gameObject, tile, radius);

        Tile gameObjectTile = gameObject.GetComponent<Tile>();

        GameObject beamGameObject = Instantiate(Beam, new Vector3((float)tile.X, (float)tile.Y, 0.0f), Quaternion.identity);
        Beam beam = beamGameObject.GetComponent<Beam>();
        beam.Distance = (tile.X - gameObjectTile.X, tile.Y - gameObjectTile.Y);
        beam.Radius = radius;
        beam.SetupSprite();
    }
}