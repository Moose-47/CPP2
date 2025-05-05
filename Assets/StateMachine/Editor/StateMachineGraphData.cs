using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace StateMachine.Editor
{
    /// <summary>
    /// Class for extracting and organizing state machine data for visualization.
    /// </summary>
    /// <typeparam name="TContext">The type of context used by the state machine</typeparam>
    public class StateMachineGraphData<TContext> where TContext : class
    {
        // Reference to the state machine
        private readonly StateMachine<TContext> stateMachine;

        // Cached data
        private Dictionary<string, StateNodeData> stateNodes = new Dictionary<string, StateNodeData>();
        private Dictionary<string, List<TransitionData>> outgoingTransitions = new Dictionary<string, List<TransitionData>>();
        private Dictionary<string, List<TransitionData>> incomingTransitions = new Dictionary<string, List<TransitionData>>();

        /// <summary>
        /// Creates a new instance of StateMachineGraphData.
        /// </summary>
        /// <param name="stateMachine">The state machine to extract data from</param>
        public StateMachineGraphData(StateMachine<TContext> stateMachine)
        {
            this.stateMachine = stateMachine;
            Update();
        }

        /// <summary>
        /// Updates the graph data from the state machine.
        /// </summary>
        public void Update()
        {
            if (stateMachine == null)
                return;

            // Clear cached data
            stateNodes.Clear();
            outgoingTransitions.Clear();
            incomingTransitions.Clear();

            // Get current and previous states
            IState<TContext> currentState = stateMachine.CurrentState;
            IState<TContext> previousState = stateMachine.PreviousState;

            // Extract state nodes
            foreach (var stateEntry in stateMachine.states)
            {
                string stateId = stateEntry.Key;
                IState<TContext> state = stateEntry.Value;

                // Create state node data
                var nodeData = new StateNodeData
                {
                    Id = stateId,
                    Name = GetStateName(state),
                    StateType = state.GetType(),
                    IsCurrentState = (state == currentState),
                    IsPreviousState = (state == previousState),
                    StateDescription = GetStateDescription(state)
                };

                // Add to dictionary
                stateNodes[stateId] = nodeData;
            }

            // Extract transitions
            foreach (var transitionEntry in stateMachine.stateTransitions)
            {
                IState<TContext> sourceState = transitionEntry.Key;
                List<Transition<TContext>> transitions = transitionEntry.Value;

                // Find source state ID
                string sourceStateId = GetStateId(sourceState);
                if (string.IsNullOrEmpty(sourceStateId))
                    continue;

                // Process each transition
                foreach (var transition in transitions)
                {
                    // Find target state ID
                    string targetStateId = GetStateId(transition.TargetState);
                    if (string.IsNullOrEmpty(targetStateId))
                        continue;

                    // Create transition data
                    var transitionData = new TransitionData
                    {
                        SourceStateId = sourceStateId,
                        TargetStateId = targetStateId,
                        Priority = transition.Priority,
                        ConditionDescription = GetConditionDescription(transition),
                        TransitionType = GetTransitionType(transition),
                        TransitionCount = transition.TransitionCount
                    };


                    // Add to outgoing transitions
                    if (!outgoingTransitions.ContainsKey(sourceStateId))
                    {
                        outgoingTransitions[sourceStateId] = new List<TransitionData>();
                    }
                    outgoingTransitions[sourceStateId].Add(transitionData);

                    // Add to incoming transitions
                    if (!incomingTransitions.ContainsKey(targetStateId))
                    {
                        incomingTransitions[targetStateId] = new List<TransitionData>();
                    }
                    incomingTransitions[targetStateId].Add(transitionData);
                }
            }
        }

        /// <summary>
        /// Gets all state nodes in the graph.
        /// </summary>
        /// <returns>A list of state node data</returns>
        public List<StateNodeData> GetAllStateNodes()
        {
            return new List<StateNodeData>(stateNodes.Values);
        }

        /// <summary>
        /// Gets the state node data for a specific state.
        /// </summary>
        /// <param name="stateId">The ID of the state</param>
        /// <returns>The state node data, or null if not found</returns>
        public StateNodeData GetStateNodeData(string stateId)
        {
            if (string.IsNullOrEmpty(stateId) || !stateNodes.ContainsKey(stateId))
                return null;

            return stateNodes[stateId];
        }

        /// <summary>
        /// Gets all transitions from a specific state.
        /// </summary>
        /// <param name="stateId">The ID of the source state</param>
        /// <returns>A list of transition data</returns>
        public List<TransitionData> GetTransitionsFromState(string stateId)
        {
            if (string.IsNullOrEmpty(stateId) || !outgoingTransitions.ContainsKey(stateId))
                return new List<TransitionData>();

            return outgoingTransitions[stateId];
        }

        /// <summary>
        /// Gets all transitions to a specific state.
        /// </summary>
        /// <param name="stateId">The ID of the target state</param>
        /// <returns>A list of transition data</returns>
        public List<TransitionData> GetTransitionsToState(string stateId)
        {
            if (string.IsNullOrEmpty(stateId) || !incomingTransitions.ContainsKey(stateId))
                return new List<TransitionData>();

            return incomingTransitions[stateId];
        }

        /// <summary>
        /// Gets the ID of a state.
        /// </summary>
        /// <param name="state">The state to get the ID for</param>
        /// <returns>The state ID, or null if not found</returns>
        private string GetStateId(IState<TContext> state)
        {
            foreach (var entry in stateMachine.states)
            {
                if (entry.Value == state)
                {
                    return entry.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a human-readable name for a state.
        /// </summary>
        /// <param name="state">The state to get the name for</param>
        /// <returns>A human-readable name</returns>
        private string GetStateName(IState<TContext> state)
        {
            // Try to get ID first
            string stateId = GetStateId(state);
            if (!string.IsNullOrEmpty(stateId))
            {
                return stateId;
            }

            // Fall back to type name
            return state.GetType().Name;
        }

        /// <summary>
        /// Gets a description of the state.
        /// </summary>
        /// <param name="state">The state to get the description for</param>
        /// <returns>A description of the state</returns>
        private string GetStateDescription(IState<TContext> state)
        {
            // Try to get description from attributes
            var attributes = state.GetType().GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attributes.Length > 0)
            {
                var descriptionAttribute = attributes[0] as DescriptionAttribute;
                if (descriptionAttribute != null)
                {
                    return descriptionAttribute.Description;
                }
            }

            // Try to get description from methods - this is not required but cool nonetheless
            //var methods = state.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //if (methods.Length > 0)
            //{
            //    string result = "Defined Methods: \n";
            //    return string.Concat(result, string.Join("\n", methods.Select(m => $"• {m.Name}()").ToArray()));
            //}

            return "No additional information available";
        }

        /// <summary>
        /// Gets a description of a transition condition.
        /// </summary>
        /// <param name="transition">The transition to get the condition description for</param>
        /// <returns>A description of the condition, or null if not available</returns>
        private string GetConditionDescription(Transition<TContext> transition)
        {
            // Use reflection to get the condition delegate's target and method
            var conditionField = typeof(Transition<TContext>).GetField("condition", BindingFlags.NonPublic | BindingFlags.Instance);
            if (conditionField != null)
            {
                var condition = conditionField.GetValue(transition) as Delegate;
                if (condition != null)
                {
                    // Get the target object and method
                    var target = condition.Target;
                    var method = condition.Method;

                    if (method != null)
                    {
                        // Check for description attribute
                        var attributes = method.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        if (attributes.Length > 0)
                        {
                            var descriptionAttribute = attributes[0] as DescriptionAttribute;
                            if (descriptionAttribute != null)
                            {
                                return descriptionAttribute.Description;
                            }
                        }

                        // If no description attribute, use method name
                        string methodName = method.Name;

                        // If it's a lambda, try to get the containing method
                        if (methodName.Contains("<") && methodName.Contains(">"))
                        {
                            // It's likely a lambda or anonymous method defined in a particular class
                            if (target != null)
                            {
                                if (target.ToString().Contains("+"))
                                {                                     
                                    // It's a lambda method in a nested class
                                    string targetName = target.ToString().Substring(0, target.ToString().IndexOf("+"));
                                    return $"Lambda in {targetName}";
                                }
                                Debug.Log($"Lambda method in {target}");
                                return $"Lambda in {target.GetType().Name}";
                            }
                            else
                            {
                                return "Lambda expression";
                            }
                        }

                        // Return method name with target type if available
                        if (target != null)
                        {
                            return $"{target.GetType().Name}.{methodName}";
                        }
                        else
                        {
                            return methodName;
                        }
                    }
                }
            }

            return "Unknown condition";
        }

        /// <summary>
        /// Gets the type of transition.
        /// </summary>
        /// <param name="transition">The transition to get the type for</param>
        /// <returns>A description of the transition type</returns>
        private string GetTransitionType(Transition<TContext> transition)
        {
            string result = "No Transition Action Defined";
            // Check if there's an action on transition
            var onTransitionField = typeof(Transition<TContext>).GetField("onTransition", BindingFlags.NonPublic | BindingFlags.Instance);
            if (onTransitionField != null)
            {
                var onTransition = onTransitionField.GetValue(transition) as Delegate;
                if (onTransition != null && onTransition.Method.Name != "lambda_method")
                {
                    var onTransitionTargetName = onTransition.Target.ToString().Substring(0, onTransition.Target.ToString().Length - 4);
                    result = $"Transition Action:\nDefined in {onTransitionTargetName}";
                }
            }

            return result;
        }

        /// <summary>
        /// Data class for state nodes in the graph.
        /// </summary>
        public class StateNodeData
        {
            /// <summary>
            /// The unique identifier of the state.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// The human-readable name of the state.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The type of the state.
            /// </summary>
            public Type StateType { get; set; }

            /// <summary>
            /// Whether this is the current active state.
            /// </summary>
            public bool IsCurrentState { get; set; }

            /// <summary>
            /// Whether this is the previous state.
            /// </summary>
            public bool IsPreviousState { get; set; }

            /// <summary>
            /// Additional description of the state.
            /// </summary>
            public string StateDescription { get; set; }
        }

        /// <summary>
        /// Data class for transitions in the graph.
        /// </summary>
        public class TransitionData
        {
            /// <summary>
            /// The ID of the source state.
            /// </summary>
            public string SourceStateId { get; set; }

            /// <summary>
            /// The ID of the target state.
            /// </summary>
            public string TargetStateId { get; set; }

            /// <summary>
            /// The priority of the transition.
            /// </summary>
            public int Priority { get; set; }

            /// <summary>
            /// A description of the transition condition.
            /// </summary>
            public string ConditionDescription { get; set; }

            /// <summary>
            /// The type of transition.
            /// </summary>
            public string TransitionType { get; set; }

            /// <summary>
            /// Counter for how many times this transition has been triggered.
            /// </summary>
            public int TransitionCount { get; set; } = 0;

        }
    }

    /// <summary>
    /// Attribute for adding descriptions to states and transition conditions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description text.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new description attribute.
        /// </summary>
        /// <param name="description">The description text</param>
        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
