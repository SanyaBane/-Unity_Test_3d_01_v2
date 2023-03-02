// using UnityEngine;
//
// namespace AnimatorBehaviours
// {
//     public class AnimBehaviourTest : StateMachineBehaviour
//     {
//         // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
//         public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//         }
//
//         // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
//         public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//         }
//
//         // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//         public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//         }
//
//         // OnStateMove is called right after Animator.OnAnimatorMove()
//         public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//             // Implement code that processes and affects root motion
//         }
//
//         // OnStateIK is called right after Animator.OnAnimatorIK()
//         public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//         {
//             // Implement code that sets up animation IK (inverse kinematics)
//         }
//
//         public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
//         {
//         }
//
//         public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
//         {
//         }
//     }
// }