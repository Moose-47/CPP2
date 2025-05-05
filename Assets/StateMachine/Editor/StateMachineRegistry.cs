using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Editor
{
    /// <summary>
    /// Static registry for tracking state machines for visualization.
    /// </summary>
    public static class StateMachineRegistry
    {
        // Dictionary of registered state machines by ID
        private static Dictionary<string, object> registeredStateMachines = new Dictionary<string, object>();
        
        // Dictionary of graph data by state machine ID
        private static Dictionary<string, object> graphDataCache = new Dictionary<string, object>();
        
        // Dictionary to store the context type for each state machine
        private static Dictionary<string, Type> contextTypes = new Dictionary<string, Type>();
        
        /// <summary>
        /// Registers a state machine for visualization.
        /// </summary>
        /// <param name="stateMachine">The state machine to register</param>
        /// <param name="id">Optional custom identifier for the state machine</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        public static void RegisterStateMachine<TContext>(StateMachine<TContext> stateMachine, string id = null) where TContext : class
        {
            if (stateMachine == null)
                return;
                
            // Generate ID if not provided
            if (string.IsNullOrEmpty(id))
            {
                id = GenerateStateMachineId(stateMachine);
            }
            
            // Register state machine
            registeredStateMachines[id] = stateMachine;
            
            // Store the context type
            contextTypes[id] = typeof(TContext);
            
            // Create graph data
            var graphData = new StateMachineGraphData<TContext>(stateMachine);
            graphDataCache[id] = graphData;
            
            Debug.Log($"State machine registered with ID: {id}");
        }
        
        /// <summary>
        /// Unregisters a state machine from visualization.
        /// </summary>
        /// <param name="id">The identifier of the state machine to unregister</param>
        public static void UnregisterStateMachine(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;
                
            // Remove state machine and graph data
            registeredStateMachines.Remove(id);
            graphDataCache.Remove(id);
            contextTypes.Remove(id);
            
            Debug.Log($"State machine unregistered with ID: {id}");
        }
        
        /// <summary>
        /// Unregisters a state machine from visualization.
        /// </summary>
        /// <param name="stateMachine">The state machine to unregister</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        public static void UnregisterStateMachine<TContext>(StateMachine<TContext> stateMachine) where TContext : class
        {
            if (stateMachine == null)
                return;
                
            // Find ID for state machine
            string id = GetStateMachineId(stateMachine);
            if (!string.IsNullOrEmpty(id))
            {
                UnregisterStateMachine(id);
            }
        }
        
        /// <summary>
        /// Gets the identifier for a registered state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to get the identifier for</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>The identifier, or null if the state machine is not registered</returns>
        public static string GetStateMachineId<TContext>(StateMachine<TContext> stateMachine) where TContext : class
        {
            if (stateMachine == null)
                return null;
                
            // Find ID for state machine
            foreach (var kvp in registeredStateMachines)
            {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (kvp.Value == stateMachine)
                {
                    return kvp.Key;
                }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }
            
            return null;
        }
        
        /// <summary>
        /// Checks if a state machine is registered for visualization.
        /// </summary>
        /// <param name="stateMachine">The state machine to check</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>True if the state machine is registered, false otherwise</returns>
        public static bool IsRegistered<TContext>(StateMachine<TContext> stateMachine) where TContext : class
        {
            return !string.IsNullOrEmpty(GetStateMachineId(stateMachine));
        }
        
        /// <summary>
        /// Gets a registered state machine by ID.
        /// </summary>
        /// <param name="id">The identifier of the state machine to get</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>The state machine, or null if not found</returns>
        public static StateMachine<TContext> GetStateMachine<TContext>(string id) where TContext : class
        {
            if (string.IsNullOrEmpty(id) || !registeredStateMachines.ContainsKey(id))
                return null;
                
            // Try to cast to correct type
            if (registeredStateMachines[id] is StateMachine<TContext> stateMachine)
            {
                return stateMachine;
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the graph data for a registered state machine.
        /// </summary>
        /// <param name="id">The identifier of the state machine</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>The graph data, or null if not found</returns>
        public static StateMachineGraphData<TContext> GetGraphData<TContext>(string id) where TContext : class
        {
            if (string.IsNullOrEmpty(id) || !graphDataCache.ContainsKey(id))
                return null;
                
            // Try to cast to correct type
            if (graphDataCache[id] is StateMachineGraphData<TContext> graphData)
            {
                // Update graph data
                var stateMachine = GetStateMachine<TContext>(id);
                if (stateMachine != null)
                {
                    graphData.Update();
                }
                
                return graphData;
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the IDs of all registered state machines.
        /// </summary>
        /// <returns>A list of state machine IDs</returns>
        public static List<string> GetRegisteredStateMachineIds()
        {
            return new List<string>(registeredStateMachines.Keys);
        }
        
        /// <summary>
        /// Generates a unique identifier for a state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine to generate an identifier for</param>
        /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
        /// <returns>A unique identifier</returns>
        private static string GenerateStateMachineId<TContext>(StateMachine<TContext> stateMachine) where TContext : class
        {
            // Use type name and instance ID
            string typeName = stateMachine.GetType().Name;
            int instanceId = stateMachine.GetInstanceID();
            
            return $"{typeName}_{instanceId}";
        }
        
        // ---- Non-generic methods for use in StateCentricWindow ----
        
        /// <summary>
        /// Gets a registered state machine by ID without requiring generic type parameter.
        /// </summary>
        /// <param name="id">The identifier of the state machine to get</param>
        /// <returns>The state machine as an object, or null if not found</returns>
        public static object GetStateMachineObject(string id)
        {
            if (string.IsNullOrEmpty(id) || !registeredStateMachines.ContainsKey(id))
                return null;
                
            return registeredStateMachines[id];
        }
        
        /// <summary>
        /// Gets the graph data for a registered state machine without requiring generic type parameter.
        /// </summary>
        /// <param name="id">The identifier of the state machine</param>
        /// <returns>The graph data as an object, or null if not found</returns>
        public static object GetGraphDataObject(string id)
        {
            if (string.IsNullOrEmpty(id) || !graphDataCache.ContainsKey(id) || !contextTypes.ContainsKey(id))
                return null;
                
            // Get the graph data object
            object graphDataObj = graphDataCache[id];
            
            // Update the graph data if possible
            Type contextType = contextTypes[id];
            if (contextType != null)
            {
                // Use reflection to call the Update method
                var updateMethod = graphDataObj.GetType().GetMethod("Update");
                if (updateMethod != null)
                {
                    updateMethod.Invoke(graphDataObj, null);
                }
            }
            
            // Return the graph data object
            return graphDataObj;
        }
    }
}
