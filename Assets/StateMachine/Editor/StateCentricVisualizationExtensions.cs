#if UNITY_EDITOR
using System;
using UnityEngine;

namespace StateMachine.Editor
{
    /// <summary>
    /// Extension methods for integrating the simplified state-centric visualization with state machines.
    /// </summary>
    public static class StateCentricVisualizationExtensions
    {
        /// <summary>
        /// Registers a state machine for visualization in the simplified state-centric view.
        /// </summary>
        /// <param name="stateMachine">The state machine to register</param>
        /// <param name="id">Optional custom identifier for the state machine</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>The state machine for method chaining</returns>
        public static StateMachine<TContext> RegisterForVisualization<TContext>(
            this StateMachine<TContext> stateMachine, 
            string id = null) where TContext : class
        {
            // Use the existing registry system
            StateMachineRegistry.RegisterStateMachine(stateMachine, id);
            
            // Enable debug mode for better visualization
            stateMachine.EnableDebug();
            
            // Log registration
            Debug.Log($"State machine registered for simplified visualization with ID: {id ?? StateMachineRegistry.GetStateMachineId(stateMachine)}");
            
            return stateMachine;
        }
        
        /// <summary>
        /// Opens the simplified state-centric visualization window for the specified state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to visualize</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>The state machine for method chaining</returns>
        public static StateMachine<TContext> OpenVisualization<TContext>(
            this StateMachine<TContext> stateMachine) where TContext : class
        {
            // Register if not already registered
            if (!StateMachineRegistry.IsRegistered(stateMachine))
            {
                stateMachine.RegisterForVisualization();
            }
            
            // Open the window
            StateCentricWindow.ShowWindow();
            
            return stateMachine;
        }
    }
}
#endif