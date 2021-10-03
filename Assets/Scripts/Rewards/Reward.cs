using UnityEngine;

public abstract class Reward
{
    public abstract void Apply(GameObject player);
    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
}
