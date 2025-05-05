using StateMachine.Editor;
using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Test script to verify the state machine implementation works correctly.
    /// </summary>
    public class StateMachineTest : MonoBehaviour
    {
        private ExampleStateMachine stateMachine;
        private ExampleContext playerContext;
        
        [SerializeField] private bool debugLogs = true;
        
        private void Start()
        {
            Debug.Log("Starting StateMachineTest...");
            
            try
            {
                // Create player context
                playerContext = new ExampleContext
                {
                    Rigidbody = GetComponent<Rigidbody2D>(),
                    Transform = transform,
                    Health = 100
                };
                
                // Add state machine component
                stateMachine = gameObject.AddComponent<ExampleStateMachine>();
                stateMachine.Initialize(playerContext);
                
                // Create states using the builder
                var stateBuilder = new StateBuilder<ExampleContext>();
                
                // Create test states with unique string identifiers
                var idleState = stateBuilder
                    .OnEnter(() => {
                        if (debugLogs) Debug.Log("Entered Idle State");
                        // Test context access
                        if (debugLogs) Debug.Log($"Context health in idle state: {playerContext.Health}");
                    })
                    .OnUpdate(() => {
                        if (playerContext.MoveInput.magnitude > 0.1f)
                        {
                            if (debugLogs) Debug.Log("Movement detected in idle state");
                            stateMachine.ChangeState("move");
                        }
                    })
                    .OnExit(() => {
                        if (debugLogs) Debug.Log("Exited Idle State");
                    })
                    .Build();
                    
                var moveState = stateBuilder
                    .OnEnter(() => {
                        if (debugLogs) Debug.Log("Entered Move State");
                        // Test context access
                        if (debugLogs) Debug.Log($"Context health in move state: {playerContext.Health}");
                    })
                    .OnUpdate(() => {
                        if (debugLogs) Debug.Log($"Moving with input: {playerContext.MoveInput}");
                        
                        if (playerContext.MoveInput.magnitude < 0.1f)
                        {
                            stateMachine.ChangeState("idle");
                        }
                    })
                    .OnExit(() => {
                        if (debugLogs) Debug.Log("Exited Move State");
                    })
                    .Build();
                    
                var jumpState = stateBuilder
                    .OnEnter(() => {
                        if (debugLogs) Debug.Log("Entered Jump State");
                        playerContext.IsJumping = true;
                        // Test context access
                        if (debugLogs) Debug.Log($"Context health in jump state: {playerContext.Health}");
                    })
                    .OnUpdate(() => {
                        
                    })
                    .OnExit(() => {
                        if (debugLogs) Debug.Log("Exited Jump State");
                        playerContext.IsJumping = false;
                    })
                    .Build();
                
                // Register states with string identifiers
                stateMachine.RegisterState("idle", idleState);
                stateMachine.RegisterState("move", moveState);
                stateMachine.RegisterState("jump", jumpState);
                
                // Set up transitions using the fluent API
                idleState.AddTransition(stateMachine)
                    .To("jump")
                    .When(() => Input.GetKeyDown(KeyCode.Space))
                    .WithAction(() => {
                        if (debugLogs) Debug.Log("Transitioning from Idle to Jump");
                        // Test context modification in transition
                        playerContext.Health -= 10;
                        if (debugLogs) Debug.Log($"Modified health in transition: {playerContext.Health}");
                    })
                    .Build();
                    
                moveState.AddTransition(stateMachine)
                    .To("jump")
                    .When(() => Input.GetKeyDown(KeyCode.Space))
                    .WithAction(() => {
                        if (debugLogs) Debug.Log("Transitioning from Move to Jump");
                        // Test context modification in transition
                        playerContext.Health -= 5;
                        if (debugLogs) Debug.Log($"Modified health in transition: {playerContext.Health}");
                    })
                    .Build();
                
                // Set initial state
                stateMachine.SetInitialState("idle");
                
                // Enable debug messaging
                StateMachineRegistry.RegisterStateMachine(stateMachine, "ExampleStateMachine");

                Debug.Log("FixedStateMachineTest initialized successfully!");
                Debug.Log("Press WASD to move, Space to jump, Tab to check current state");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in StateMachineTest: {e.Message}\n{e.StackTrace}");
            }
        }
        
        private void Update()
        {
            try
            {
                // Update input in the context
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                playerContext.MoveInput = new Vector2(horizontal, vertical);
                
                // Debug current state and context
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Debug.Log($"Current state: {stateMachine.CurrentState.GetType().Name}");
                    Debug.Log($"Current health: {playerContext.Health}");
                    Debug.Log($"Is jumping: {playerContext.IsJumping}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in Update: {e.Message}");
            }
        }
    }
}
