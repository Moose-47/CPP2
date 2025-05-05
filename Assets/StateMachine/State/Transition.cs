using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Represents a transition between states.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the states</typeparam>
    public class Transition<TContext> where TContext : class
    {
        private int transitionCount = 0;
        private readonly IState<TContext> targetState;
        private readonly Func<bool> condition;
        private readonly Action onTransition;
        private readonly int priority;

        /// <summary>
        /// Gets the number of times this transition has been triggered.
        /// </summary>
        public int TransitionCount => transitionCount;
        
        /// <summary>
        /// Gets the target state of the transition.
        /// </summary>
        public IState<TContext> TargetState => targetState;
        
        /// <summary>
        /// Gets the priority of the transition.
        /// </summary>
        public int Priority => priority;
        
        /// <summary>
        /// Constructor for creating a transition.
        /// </summary>
        /// <param name="targetState">The state to transition to</param>
        /// <param name="condition">Function that determines if transition should occur</param>
        /// <param name="onTransition">Optional action to perform during transition</param>
        /// <param name="priority">Priority of this transition</param>
        public Transition(
            IState<TContext> targetState,
            Func<bool> condition,
            Action onTransition = null,
            int priority = 0)
        {
            this.targetState = targetState;
            this.condition = condition;
            this.onTransition = onTransition; //?? (() => {});
            this.priority = priority;
        }

        /// <summary>
        /// Determines if the transition should occur.
        /// </summary>
        /// <returns>True if the transition should occur, false otherwise</returns>
        public bool ShouldTransition()
        {
            bool shouldTransition = condition();
            if (shouldTransition)
            {
                transitionCount++;

                // Uncomment the following lines to enable debug logging for transitions
                //string DebugMsg;
                //LambdaState<TContext> targetLambdaState = targetState as LambdaState<TContext>;
                //DebugMsg = (targetLambdaState != null) ? $"Transition to {targetLambdaState.Name} triggered. Count: {transitionCount}" : $"Transition to {targetState.GetType().Name} triggered. Count: {transitionCount}";
                //Debug.Log(DebugMsg);
            }
            return shouldTransition;
        }


        /// <summary>
        /// Performs the transition action.
        /// </summary>
        public void PerformTransitionAction()
        {
            if (onTransition != null)
            {
                onTransition();
            }
        }
    }
}
