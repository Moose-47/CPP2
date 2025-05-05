using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// A fluent API for building states using lambda expressions.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the state</typeparam>
    public class StateBuilder<TContext> where TContext : class
    {
        private string name;
        private Action onEnter;
        private Action onExit;
        private Action onUpdate;
        private Action onFixedUpdate;
        
        /// <summary>
        /// Constructor for creating a state builder.
        /// </summary>
        public StateBuilder()
        {
            onEnter = () => {};
            onExit = () => {};
            onUpdate = () => {};
            onFixedUpdate = () => {};
        }
        
        /// <summary>
        /// Sets the action to perform when entering the state.
        /// </summary>
        /// <param name="action">Action to perform when entering the state</param>
        /// <returns>The builder for method chaining</returns>
        public StateBuilder<TContext> OnEnter(Action action)
        {
            onEnter = action ?? onEnter;
            return this;
        }
        
        /// <summary>
        /// Sets the action to perform when exiting the state.
        /// </summary>
        /// <param name="action">Action to perform when exiting the state</param>
        /// <returns>The builder for method chaining</returns>
        public StateBuilder<TContext> OnExit(Action action)
        {
            onExit = action ?? onExit;
            return this;
        }
        
        /// <summary>
        /// Sets the action to perform during the update loop.
        /// </summary>
        /// <param name="action">Action to perform during the update loop</param>
        /// <returns>The builder for method chaining</returns>
        public StateBuilder<TContext> OnUpdate(Action action)
        {
            onUpdate = action ?? onUpdate;
            return this;
        }
        
        /// <summary>
        /// Sets the action to perform during the fixed update loop.
        /// </summary>
        /// <param name="action">Action to perform during the fixed update loop</param>
        /// <returns>The builder for method chaining</returns>
        public StateBuilder<TContext> OnFixedUpdate(Action action)
        {
            onFixedUpdate = action ?? onFixedUpdate;
            return this;
        }

        /// <summary>
        /// Sets the action to perform when entering the state.
        /// </summary>
        /// <param name="action">Action to perform when entering the state</param>
        /// <returns>The builder for method chaining</returns>
        public StateBuilder<TContext> SetName(string name)
        {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Builds and returns the state.
        /// </summary>
        /// <returns>The built state</returns>
        public IState<TContext> Build()
        {
            return new LambdaState<TContext>(name, onEnter, onExit, onUpdate, onFixedUpdate);
        }
    }
}
