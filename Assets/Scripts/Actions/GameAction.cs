﻿using UnityEngine;

public abstract class GameAction
{
    public GameObject GameObject { get; set; }

    public virtual void Apply(GameMap gameMap)
    {
        IsDone = false;
    }

    public virtual void Update(GameMap gameMap) { }

    public bool IsDone { get; protected set; }
}