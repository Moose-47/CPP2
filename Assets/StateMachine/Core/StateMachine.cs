using System;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.Editor;

namespace StateMachine
{
    /// <summary>
    /// Unity-specific state machine implementation that manages states and transitions.
    /// </summary>
    /// <typeparam name="TContext">The type of context data used by the states</typeparam>
    public abstract class StateMachine<TContext> : MonoBehaviour where TContext : class
    {
        // Current active state
        private IState<TContext> currentState;
        
        // Previous state for history functionality
        private IState<TContext> previousState;
        
        // Context data shared between states
        [SerializeField] protected TContext context;
        
        // Dictionary of all registered states with string identifiers
        public Dictionary<string, IState<TContext>> states = new Dictionary<string, IState<TContext>>();
        
        // Transitions for each state
        public Dictionary<IState<TContext>, List<Transition<TContext>>> stateTransitions = 
            new Dictionary<IState<TContext>, List<Transition<TContext>>>();
            
        // Debug messages
        [SerializeField] private bool debugMode = false;
        
        // Event fired when state changes
        public event Action<IState<TContext>, IState<TContext>> OnStateChanged;
        
        /// <summary>
        /// Gets the current state.
        /// </summary>
        public IState<TContext> CurrentState => currentState;
        
        /// <summary>
        /// Gets the previous state.
        /// </summary>
        public IState<TContext> PreviousState => previousState;
        
        /// <summary>
        /// Gets the context data.
        /// </summary>
        public TContext Context => context;


        private void OnEnable()
        {
            if (debugMode)
            {
                // Register this state machine for visualization
                StateMachineRegistry.RegisterStateMachine(this);
            }   
        }

        private void OnDisable()
        {
            // Unregister this state machine for visualization
            StateMachineRegistry.UnregisterStateMachine(this);
        }


        /// <summary>
        /// Initialize the state machine.
        /// </summary>
        /// <param name="context">The context data</param>
        public void Initialize(TContext context)
        {
            this.context = context;
        }
        
        /// <summary>
        /// Registers a state with the state machine using a unique identifier.
        /// </summary>
        /// <param name="stateId">Unique identifier for the state</param>
        /// <param name="state">The state to register</param>
        /// <typeparam name="T">The type of state</typeparam>
        /// <returns>The registered state for method chaining</returns>
        public T RegisterState<T>(string stateId, T state) where T : IState<TContext>
        {
            if (states.ContainsKey(stateId))
            {
                throw new InvalidOperationException($"State with ID '{stateId}' is already registered");
            }
            
            // Set references to this state machine and the shared context
            state.SetReferences(this, context);
            
            states[stateId] = state;
            stateTransitions[state] = new List<Transition<TContext>>();
            
            return state;
        }
        
        /// <summary>
        /// Gets a state by its unique identifier.
        /// </summary>
        /// <param name="stateId">The unique identifier of the state</param>
        /// <returns>The state with the specified identifier</returns>
        public IState<TContext> GetState(string stateId)
        {
            if (!states.ContainsKey(stateId))
            {
                throw new InvalidOperationException($"State with ID '{stateId}' is not registered");
            }
            
            return states[stateId];
        }
        
        /// <summary>
        /// Adds a transition between states.
        /// </summary>
        /// <param name="fromState">The source state</param>
        /// <param name="toState">The target state</param>
        /// <param name="condition">Function that determines if transition should occur</param>
        /// <param name="onTransition">Optional action to perform during transition</param>
        /// <param name="priority">Priority of this transition</param>
        /// <returns>The created transition for method chaining</returns>
        public Transition<TContext> AddTransition(
            IState<TContext> fromState,
            IState<TContext> toState,
            Func<bool> condition,
            Action onTransition = null,
            int priority = 0)
        {
            if (!stateTransitions.ContainsKey(fromState))
            {
                throw new InvalidOperationException($"State {fromState.GetType().Name} is not registered");
            }
            
            var transition = new Transition<TContext>(toState, condition, onTransition, priority);
            stateTransitions[fromState].Add(transition);
            
            // Sort transitions by priority (descending)
            stateTransitions[fromState].Sort((a, b) => b.Priority.CompareTo(a.Priority));
                
            return transition;
        }
        
        /// <summary>
        /// Sets the initial state of the state machine.
        /// </summary>
        /// <param name="stateId">The unique identifier of the state to set as initial</param>
        public void SetInitialState(string stateId)
        {
            if (!states.ContainsKey(stateId))
            {
                throw new InvalidOperationException($"State with ID '{stateId}' is not registered");
            }
            
            currentState = states[stateId];
            currentState.Enter();
            
            if (debugMode)
            {
                Debug.Log($"Initial state set to: {stateId}");
            }
        }
        
        /// <summary>
        /// Sets the initial state of the state machine.
        /// </summary>
        /// <param name="state">The state to set as initial</param>
        public void SetInitialState(IState<TContext> state)
        {
            if (!stateTransitions.ContainsKey(state))
            {
                throw new InvalidOperationException($"State {state.GetType().Name} is not registered");
            }
            
            currentState = state;
            currentState.Enter();
            
            if (debugMode)
            {
                Debug.Log($"Initial state set to: {state.ToString()}");
            }
        }
        
        /// <summary>
        /// Changes the current state to the specified state.
        /// </summary>
        /// <param name="stateId">The unique identifier of the state to change to</param>
        public void ChangeState(string stateId)
        {
            if (!states.ContainsKey(stateId))
            {
                throw new InvalidOperationException($"State with ID '{stateId}' is not registered");
            }
            
            ChangeState(states[stateId]);
        }
        
        /// <summary>
        /// Changes the current state to the specified state.
        /// </summary>
        /// <param name="newState">The state to change to</param>
        public void ChangeState(IState<TContext> newState)
        {
            if (!stateTransitions.ContainsKey(newState))
            {
                throw new InvalidOperationException($"State {newState.GetType().Name} is not registered");
            }
            
            // Store previous state
            previousState = currentState;
            
            // Exit current state
            if (currentState != null)
            {
                currentState.Exit();
            }
            
            // Set and enter new state
            var oldState = currentState;
            currentState = newState;
            currentState.Enter();
            
            // Notify listeners
            OnStateChanged?.Invoke(oldState, currentState);
            
            if (debugMode)
            {
                LambdaState<TContext> oldLambda = oldState as LambdaState<TContext>;
                LambdaState<TContext> newLambda = newState as LambdaState<TContext>;
                Debug.Log($"State changed from {(oldLambda != null ? oldLambda.Name : oldState != null ? oldState.GetType().Name : "null")} to {(newLambda != null ? newLambda.Name : newState != null ? newState.GetType().Name : "null")}");
            }
        }
        
        /// <summary>
        /// Returns to the previous state.
        /// </summary>
        public void RevertToPreviousState()
        {
            if (previousState != null)
            {
                ChangeState(previousState);
            }
        }
        
        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (currentState == null) return;
            
            // Check for transitions
            CheckTransitions();
            
            // Update current state
            currentState.Update();
        }

        /// <summary>
        /// FixedUpdate is called once per physics update.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (currentState == null) return;
            
            currentState.FixedUpdate();
        }
        
        /// <summary>
        /// Checks if any transitions should occur from the current state.
        /// </summary>
        private void CheckTransitions()
        {
            if (currentState == null) return;
            
            var transitions = stateTransitions[currentState];
            
            foreach (var transition in transitions)
            {
                if (transition.ShouldTransition())
                {
                    // Perform transition action
                    transition.PerformTransitionAction();
                    
                    // Change state
                    ChangeState(transition.TargetState);
                    
                    // Only one transition per update
                    break;
                }
            }
        }
        
        /// <summary>
        /// Enables debug messages for the state machine.
        /// </summary>
        public void EnableDebug()
        {
            debugMode = true;
        }
    }
}
