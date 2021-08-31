using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AIStateTurn
{
    public AIState AIState;
    public int NbTurns = 1;
}

[CreateAssetMenu(fileName = "NewAIBehavior", menuName = "AI/AIBehavior")]
public class AIBehavior : ScriptableObject
{
    public List<AIStateTurn> AIStateTurns;
}