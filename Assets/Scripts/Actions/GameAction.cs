using UnityEngine;

public abstract class GameAction
{
    public GameObject GameObject { get; set; }

    public virtual void Apply(GameMap gameMap)
    {
        IsDone = false;
        IsSuccess = true;
        IsPausing = false;
    }

    public virtual void Update(GameMap gameMap) { }

    public virtual void Release(GameMap gameMap) { }

    public bool IsDone { get; protected set; }

    public bool IsSuccess { get; protected set; }

    public bool IsPausing { get; protected set; }

    public virtual string GetDebugString() { return null; }
}