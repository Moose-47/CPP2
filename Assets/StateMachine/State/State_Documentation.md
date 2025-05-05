# State Folder Documentation

The State folder contains the components related to state definitions, transitions, and state building in the StateMachine system. This folder provides the building blocks for creating and connecting states in a state machine.

## Files Overview

### IState.cs

This file defines the `IState<TContext>` interface, which is the foundation for all state implementations in the system.

**Key Features:**
- Generic interface that works with any context type
- Defines the core lifecycle methods for states
- Provides a contract for state implementations

**Main Components:**
- `Enter()` - Called when entering the state
- `Exit()` - Called when exiting the state
- `Update()` - Called during the update loop
- `FixedUpdate()` - Called during the fixed update loop
- `SetReferences()` - Sets the state machine and context references

**Usage Example:**
```csharp
// Create a custom state by implementing the IState interface
public class PlayerIdleState : IState<PlayerContext>
{
    private StateMachine<PlayerContext> stateMachine;
    private PlayerContext context;
    
    public void SetReferences(StateMachine<PlayerContext> stateMachine, PlayerContext context)
    {
        this.stateMachine = stateMachine;
        this.context = context;
    }
    
    public void Enter() { /* Initialization code */ }
    public void Exit() { /* Cleanup code */ }
    public void Update() { /* Per-frame logic */ }
    public void FixedUpdate() { /* Physics-based logic */ }
}
```

### StateBase.cs

This file provides the `StateBase<TContext>` abstract class, which implements the `IState<TContext>` interface with default behavior.

**Key Features:**
- Provides a base implementation for states
- Handles reference management for state machine and context
- Offers virtual methods for easy overriding

**Main Components:**
- Protected fields for state machine and context access
- Default implementations of lifecycle methods
- Implementation of the `SetReferences()` method

**Usage Example:**
```csharp
// Create a custom state by inheriting from StateBase
public class PlayerIdleState : StateBase<PlayerContext>
{
    public override void Enter()
    {
        // Access context and state machine through protected fields
        context.IsMoving = false;
    }
    
    public override void Update()
    {
        // Check for transition conditions
        if (context.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState("move");
        }
    }
}
```

### LambdaState.cs

This file defines the `LambdaState<TContext>` class, which allows for creating states using lambda expressions.

**Key Features:**
- Creates states without defining new classes
- Uses lambda expressions for state behavior
- Supports naming states for better debugging

**Main Components:**
- Constructor accepting lambda expressions for each lifecycle method
- Implementation of the `IState<TContext>` interface
- Optional name property for identification

**Usage Example:**
```csharp
// Create a state using lambda expressions
var idleState = new LambdaState<PlayerContext>(
    name: "Idle",
    onEnter: () => { Debug.Log("Entered Idle State"); },
    onUpdate: () => {
        if (context.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState("move");
        }
    },
    onExit: () => { Debug.Log("Exited Idle State"); }
);
```

### Transition.cs

This file defines the `Transition<TContext>` class, which represents a transition between states.

**Key Features:**
- Encapsulates transition logic and conditions
- Supports priority-based transition ordering
- Allows for custom actions during transitions
- Tracks how many times each transition has been triggered

**Main Components:**
- Target state reference
- Condition function for determining when to transition
- Optional action to perform during transition
- Priority value for handling multiple valid transitions
- Transition counter that increments when a transition is triggered

**Usage Example:**
```csharp
// Create a transition between states
var transition = new Transition<PlayerContext>(
    targetState: runState,
    condition: () => context.MoveInput.magnitude > 0.1f,
    onTransition: () => { Debug.Log("Transitioning to Run state"); },
    priority: 1
);

// The transition counter automatically increments each time 
// the transition condition returns true and the transition is taken
```

## Builder Subfolder

The Builder subfolder contains classes that implement a fluent API for building states and transitions.

### StateBuilder.cs

This file defines the `StateBuilder<TContext>` class, which provides a fluent API for creating states.

**Key Features:**
- Method chaining for defining state behavior
- Creates `LambdaState<TContext>` instances
- Simplifies state creation without subclassing

**Main Components:**
- Methods for setting enter, exit, update, and fixed update actions
- Build method for creating the final state
- Support for naming states

**Usage Example:**
```csharp
// Create a state using the builder
var idleState = new StateBuilder<PlayerContext>()
    .SetName("Idle")
    .OnEnter(() => { Debug.Log("Entered Idle State"); })
    .OnUpdate(() => {
        if (context.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState("move");
        }
    })
    .OnExit(() => { Debug.Log("Exited Idle State"); })
    .Build();
```

### TransitionBuilder.cs

This file defines the `TransitionBuilder<TContext>` class and extension methods for creating transitions with a fluent API.

**Key Features:**
- Method chaining for defining transitions
- Creates `Transition<TContext>` instances
- Integrates with the state machine for automatic registration

**Main Components:**
- Methods for setting target state, condition, action, and priority
- Build method for creating and registering the transition
- Extension methods for starting from a state

**Usage Example:**
```csharp
// Create a transition using the builder
idleState.AddTransition(stateMachine)
    .To("run")
    .When(() => context.MoveInput.magnitude > 0.1f)
    .WithAction(() => { Debug.Log("Transitioning to Run state"); })
    .WithPriority(1)
    .Build();
```

## State Folder Summary

The State folder provides the components needed to define states and transitions in the state machine system:

1. The `IState<TContext>` interface defines the contract for states
2. The `StateBase<TContext>` class provides a base implementation for custom states
3. The `LambdaState<TContext>` class enables creating states with lambda expressions
4. The `Transition<TContext>` class represents transitions between states and tracks transition counts
5. The Builder subfolder provides fluent APIs for creating states and transitions

These components work together to create a flexible and expressive system for defining state machines with minimal boilerplate code.
