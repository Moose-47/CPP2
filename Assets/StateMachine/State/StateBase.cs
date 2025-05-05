using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Base class for states that need access to the context and state machine.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the state</typeparam>
    public abstract class StateBase<TContext> : IState<TContext> where TContext : class
    {
        // References to the state machine and context
        protected StateMachine<TContext> stateMachine;
        protected TContext context;

        ///// <summary>
        ///// Sets the state machine and context reference for this state. Called from the RegisterState function.
        ///// </summary>
        ///// <param name="stateMachine">The state machine this state belongs to</param>
        ///// <param name="context">The shared context data</param>
        public void SetReferences(StateMachine<TContext> stateMachine, TContext context)
        {
            this.stateMachine = stateMachine;
            this.context = context;
        }
        
        /// <summary>
        /// Called when entering the state.
        /// </summary>
        public virtual void Enter() { }
        
        /// <summary>
        /// Called when exiting the state.
        /// </summary>
        public virtual void Exit() { }
        
        /// <summary>
        /// Called during the update loop.
        /// </summary>
        public virtual void Update() { }
        
        /// <summary>
        /// Called during the fixed update loop.
        /// </summary>
        public virtual void FixedUpdate() { }
    }
}
