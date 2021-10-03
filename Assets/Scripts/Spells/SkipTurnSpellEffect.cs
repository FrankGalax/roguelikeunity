using UnityEngine;

[CreateAssetMenu(fileName = "NewSkipTurnSpellEffect", menuName = "Spells/SkipTurnSpellEffect")]
public class SkipTurnSpellEffect : SpellEffect
{
    public int Turns = 1;

    public override void TargetActor(GameObject gameObject, Tile actor)
    {
        base.TargetActor(gameObject, actor);

        AIComponent aiComponent = actor.GetComponent<AIComponent>();
        if (aiComponent != null)
        {
            aiComponent.SkipTurns += Turns;
        }
    }
}