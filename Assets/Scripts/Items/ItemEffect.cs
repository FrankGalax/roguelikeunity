using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Apply(GameObject gameObject, GameMap gameMap);
}
