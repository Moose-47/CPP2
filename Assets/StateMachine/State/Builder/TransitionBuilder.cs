using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// A fluent API for building transitions between states.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the states</typeparam>
    public class TransitionBuilder<TContext> where TContext : class
    {
        private readonly StateMachine<TContext> stateMachine;
        private readonly IState<TContext> fromState;
        private IState<TContext> toState;
        private Func<bool> condition;
        private Action action;
        private int priority;
        
        /// <summary>
        /// Constructor for creating a transition builder.
        /// </summary>
        /// <param name="stateMachine">The state machine to add the transition to</param>
        /// <param name="fromState">The source state</param>
        public TransitionBuilder(StateMachine<TContext> stateMachine, IState<TContext> fromState)
        {
            this.stateMachine = stateMachine;
            this.fromState = fromState;
            this.priority = 0;
        }
        
        /// <summary>
        /// Sets the target state for the transition.
        /// </summary>
        /// <param name="state">The state to transition to</param>
        /// <returns>The builder for method chaining</returns>
        public TransitionBuilder<TContext> To(IState<TContext> state)
        {
            this.toState = state;
            return this;
        }
        
        /// <summary>
        /// Sets the target state for the transition by ID.
        /// </summary>
        /// <param name="stateId">The ID of the state to transition to</param>
        /// <returns>The builder for method chaining</returns>
        public TransitionBuilder<TContext> To(string stateId)
        {
            this.toState = stateMachine.GetState(stateId);
            return this;
        }
        
        /// <summary>
        /// Sets the condition for the transition.
        /// </summary>
        /// <param name="condition">Function that determines if transition should occur</param>
        /// <returns>The builder for method chaining</returns>
        public TransitionBuilder<TContext> When(Func<bool> condition)
        {
            this.condition = condition;
            return this;
        }
        
        /// <summary>
        /// Sets the action to perform during the transition.
        /// </summary>
        /// <param name="action">Action to perform during transition</param>
        /// <returns>The builder for method chaining</returns>
        public TransitionBuilder<TContext> WithAction(Action action)
        {
            this.action = action;
            return this;
        }
        
        /// <summary>
        /// Sets the priority of the transition.
        /// </summary>
        /// <param name="priority">Priority of this transition</param>
        /// <returns>The builder for method chaining</returns>
        public TransitionBuilder<TContext> WithPriority(int priority)
        {
            this.priority = priority;
            return this;
        }
        
        /// <summary>
        /// Builds and adds the transition to the state machine.
        /// </summary>
        /// <returns>The created transition</returns>
        public Transition<TContext> Build()
        {
            if (toState == null)
            {
                throw new InvalidOperationException("Target state must be specified");
            }
            
            if (condition == null)
            {
                throw new InvalidOperationException("Transition condition must be specified");
            }
            
            return stateMachine.AddTransition(fromState, toState, condition, action, priority);
        }
    }
    
    /// <summary>
    /// Extension methods for IState to add transitions with a fluent API.
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        /// Starts building a transition from this state.
        /// </summary>
        /// <typeparam name="TContext">The type of context data used by the states</typeparam>
        /// <param name="state">The source state</param>
        /// <param name="stateMachine">The state machine to add the transition to</param>
        /// <returns>A transition builder for fluent configuration</returns>
        public static TransitionBuilder<TContext> AddTransition<TContext>(
            this IState<TContext> state,
            StateMachine<TContext> stateMachine) where TContext : class
        {
            return new TransitionBuilder<TContext>(stateMachine, state);
        }
    }
}
