using UnityEngine;

namespace Project.Game.Systems
{
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        // OnStateExit se llama cuando una transición termina y la máquina de estados finaliza de evaluar este estado.
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var player = animator.GetComponentInParent<Player.Scripts.Player>();
            if (player != null)
            {
                player.OnAttackFinished();
            }
        }
    }
}