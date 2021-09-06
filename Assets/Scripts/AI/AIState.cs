using UnityEngine;

public abstract class AIState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        AIComponent aiComponent = animator.GetComponent<AIComponent>();
        aiComponent.GetActionFunc = GetAction;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        /*AIComponent aiComponent = animator.GetComponent<AIComponent>();
        aiComponent.GetActionFunc = null;*/
    }

    public abstract GameAction GetAction(GameObject gameObject, GameObject player, GameMap gameMap);
}