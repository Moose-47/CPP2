using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// A simple state implementation that uses lambda expressions for its behavior.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the state</typeparam>
    public class LambdaState<TContext> : IState<TContext> where TContext : class
    {
        public string Name { get; private set; }
        private readonly Action onEnter;
        private readonly Action onExit;
        private readonly Action onUpdate;
        private readonly Action onFixedUpdate;
        
        // References to the state machine and context
        protected StateMachine<TContext> stateMachine;
        protected TContext context;
        
        /// <summary>
        /// Constructor for creating a lambda state.
        /// </summary>
        /// <param name="onEnter">Action to perform when entering the state</param>
        /// <param name="onExit">Action to perform when exiting the state</param>
        /// <param name="onUpdate">Action to perform during the update loop</param>
        /// <param name="onFixedUpdate">Action to perform during the fixed update loop</param>
        public LambdaState(
            string name = null,
            Action onEnter = null,
            Action onExit = null,
            Action onUpdate = null,
            Action onFixedUpdate = null)
        {
            this.Name = name;
            this.onEnter = onEnter ?? (() => {});
            this.onExit = onExit ?? (() => {});
            this.onUpdate = onUpdate ?? (() => {});
            this.onFixedUpdate = onFixedUpdate ?? (() => {});
        }
        
        /// <summary>
        /// Sets the state machine and context reference for this state.
        /// </summary>
        /// <param name="stateMachine">The state machine this state belongs to</param>
        /// <param name="context">The shared context data</param>
        public void SetReferences(StateMachine<TContext> stateMachine, TContext context)
        {
            this.stateMachine = stateMachine;
            this.context = context;
        }
        
        /// <summary>
        /// Called when entering the state.
        /// </summary>
        public void Enter()
        {
            onEnter();
        }
        
        /// <summary>
        /// Called when exiting the state.
        /// </summary>
        public void Exit()
        {
            onExit();
        }
        
        /// <summary>
        /// Called during the update loop.
        /// </summary>
        public void Update()
        {
            onUpdate();
        }
        
        /// <summary>
        /// Called during the fixed update loop.
        /// </summary>
        public void FixedUpdate()
        {
            onFixedUpdate();
        }
    }
}
