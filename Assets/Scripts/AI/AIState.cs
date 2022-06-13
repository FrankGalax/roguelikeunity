using UnityEngine;

public abstract class AIState : StateMachineBehaviour
{
    public Effect Effect;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        AIComponent aiComponent = animator.GetComponent<AIComponent>();
        aiComponent.GetActionFunc = GetAction;

        if (Effect != null)
        {
            EffectComponent effectComponent = animator.GetComponent<EffectComponent>();
            if (effectComponent != null)
            {
                effectComponent.AddEffect(Effect, -1, animator.gameObject);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        /*AIComponent aiComponent = animator.GetComponent<AIComponent>();
        aiComponent.GetActionFunc = null;*/

        if (Effect != null)
        {
            EffectComponent effectComponent = animator.GetComponent<EffectComponent>();
            if (effectComponent != null)
            {
                effectComponent.RemoveEffect(Effect);
            }
        }
    }

    public abstract GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap);
}