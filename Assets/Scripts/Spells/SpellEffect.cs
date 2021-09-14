using UnityEngine;

public abstract class SpellEffect : ScriptableObject
{
    public virtual void TargetActor(GameObject gameObject, Tile actor) { }

    public virtual void TargetTile(GameObject gameObject, Tile tile) { }
}