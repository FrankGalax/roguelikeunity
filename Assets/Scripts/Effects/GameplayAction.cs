using UnityEngine;

public abstract class GameplayAction : ScriptableObject
{
    public abstract GameplayActionInstance CreateInstance();
}