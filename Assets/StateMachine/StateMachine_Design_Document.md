# State Machine Design Document

## Overview

The Cat Splat Studios State Machine is a flexible, generic implementation of the State pattern for Unity game development. It provides a robust framework for managing game states, transitions between states, and the data shared between them. This document explains the overall design of the state machine system and how to use it effectively in your Unity projects.

## Core Architecture

The state machine system is built around several key components that work together:

### 1. State Machine

The `StateMachine<TContext>` class is the central component that manages states and transitions. It inherits from MonoBehaviour, allowing it to be attached to GameObjects and participate in Unity's lifecycle.

Key responsibilities:
- Maintaining a collection of states
- Managing transitions between states
- Updating the current state during Unity's update cycles
- Providing events for state changes

### 2. Context

The context is a user-defined class that contains the data shared between states. The system provides a `ReactiveContext<T>` base class that implements property change notifications, but any class can be used as a context.

Key features:
- Holds shared data accessible to all states
- Can notify listeners when properties change (when using `ReactiveContext<T>`)
- Typically contains references to components, input data, and game state

### 3. States

States represent distinct behaviors or modes in your game. The system provides several ways to define states:

- Implementing the `IState<TContext>` interface
- Extending the `StateBase<TContext>` abstract class
- Using the `LambdaState<TContext>` class with lambda expressions
- Using the `StateBuilder<TContext>` fluent API

Each state has lifecycle methods:
- `Enter()` - Called when the state becomes active
- `Exit()` - Called when the state is no longer active
- `Update()` - Called during Unity's Update
- `FixedUpdate()` - Called during Unity's FixedUpdate

### 4. Transitions

Transitions define when and how states change. The `Transition<TContext>` class encapsulates:

- A target state
- A condition function that determines when to transition
- An optional action to perform during the transition
- A priority value for handling multiple valid transitions
- A counter that tracks how many times the transition has been triggered

Transitions can be created directly or using the `TransitionBuilder<TContext>` fluent API.

## Design Patterns

The state machine system incorporates several design patterns:

### State Pattern

The core of the system is the State pattern, which allows an object to alter its behavior when its internal state changes. This is implemented through the `IState<TContext>` interface and its implementations.

### Builder Pattern

The system uses the Builder pattern through the `StateBuilder<TContext>` and `TransitionBuilder<TContext>` classes, providing a fluent API for creating states and transitions.

### Observer Pattern

The `ReactiveContext<T>` class implements the Observer pattern, allowing states to react to changes in the shared context data.

### Dependency Injection

States receive references to the state machine and context through the `SetReferences()` method, implementing a form of dependency injection.

## Usage Workflow

### 1. Define Your Context

Create a class to hold the shared data for your state machine:

```csharp
public class PlayerContext : ReactiveContext<PlayerContext>
{
    // References to components
    public Rigidbody2D Rigidbody { get; set; }
    public Animator Animator { get; set; }
    
    // State properties with change notification
    private bool isGrounded;
    public bool IsGrounded
    {
        get => isGrounded;
        set => SetProperty(ref isGrounded, value, nameof(IsGrounded));
    }
    
    // Input properties
    public Vector2 MoveInput { get; set; }
}
```

### 2. Create Your State Machine

Create a class that inherits from `StateMachine<TContext>`:

```csharp
public class PlayerStateMachine : StateMachine<PlayerContext>
{
    // You can add custom methods or properties specific to your game
}
```

### 3. Define States

Define states using one of the available approaches:

**Using StateBuilder (recommended for simple states):**

```csharp
var idleState = new StateBuilder<PlayerContext>()
    .SetName("Idle")
    .OnEnter(() => {
        context.Animator.SetBool("IsMoving", false);
    })
    .OnUpdate(() => {
        // Check for transition conditions in Update
    })
    .OnExit(() => {
        // Cleanup when leaving the state
    })
    .Build();
```

**Using custom classes (recommended for complex states):**

```csharp
public class PlayerIdleState : StateBase<PlayerContext>
{
    public override void Enter()
    {
        context.Animator.SetBool("IsMoving", false);
    }
    
    public override void Update()
    {
        if (context.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState("run");
        }
    }
}
```

### 4. Register States

Register your states with the state machine:

```csharp
// Using string identifiers (recommended)
stateMachine.RegisterState("idle", idleState);
stateMachine.RegisterState("run", runState);

// Or using variables directly
var jumpState = new PlayerJumpState();
stateMachine.RegisterState("jump", jumpState);
```

### 5. Define Transitions

Define transitions between states:

**Using the fluent API (recommended):**

```csharp
idleState.AddTransition(stateMachine)
    .To("run")
    .When(() => context.MoveInput.magnitude > 0.1f)
    .WithAction(() => {
        // Optional action during transition
    })
    .WithPriority(1)
    .Build();
```

**Or using the direct method:**

```csharp
stateMachine.AddTransition(
    idleState,
    runState,
    () => context.MoveInput.magnitude > 0.1f,
    () => { /* Optional action */ },
    1 // Priority
);
```

### 6. Set Initial State

Set the initial state of the state machine:

```csharp
stateMachine.SetInitialState("idle");
```

### 7. Initialize Context

Initialize the context with necessary references:

```csharp
void Start()
{
    var context = new PlayerContext
    {
        Rigidbody = GetComponent<Rigidbody2D>(),
        Animator = GetComponent<Animator>()
    };
    
    stateMachine.Initialize(context);
}
```

### 8. Enable Visualization (Optional)

Register the state machine for visualization in the editor:

```csharp
stateMachine.RegisterForVisualization("PlayerStateMachine");
```

## Best Practices

### State Design

1. **Keep states focused**: Each state should represent a single behavior or mode.
2. **Use the context for shared data**: Avoid storing state in the states themselves.
3. **Prefer composition over inheritance**: Use the builder pattern for simple states and custom classes for complex ones.
4. **Use meaningful names**: Give states and transitions descriptive names for better debugging.

### Transitions

1. **Prioritize transitions**: Use the priority parameter to ensure the most important transitions happen first.
2. **Keep conditions simple**: Transition conditions should be clear and focused.
3. **Use transition actions sparingly**: Only use transition actions for operations that are truly part of the transition.
4. **Monitor transition usage**: Use the transition counter to identify which transitions are being triggered most frequently.

### Context Design

1. **Use reactive properties**: Take advantage of the `ReactiveContext<T>` for property change notifications.
2. **Separate input handling**: Update the context with input data, but let states decide how to respond.
3. **Include necessary references**: Store references to components and other objects in the context.

### Debugging

1. **Enable debug mode**: Call `stateMachine.EnableDebug()` to see state changes in the console.
2. **Use visualization**: Register the state machine for visualization to see states and transitions in the editor.
3. **Add descriptions**: Use the `DescriptionAttribute` to add descriptions to states and methods.
4. **Track transition counts**: Use the transition counter feature to monitor how often transitions are triggered.

## Advanced Features

### State History

The state machine keeps track of the previous state, allowing you to revert to it:

```csharp
stateMachine.RevertToPreviousState();
```

### Event Notifications

Subscribe to state change events:

```csharp
stateMachine.OnStateChanged += (oldState, newState) => {
    Debug.Log($"State changed from {oldState} to {newState}");
};
```

### Reactive Properties

Use the reactive context to respond to property changes:

```csharp
context.OnPropertyChanged(nameof(PlayerContext.Health), () => {
    // React to health changes
});
```

### Transition Counting

The state machine automatically tracks how many times each transition is triggered:

```csharp
// The transition counter increments automatically when a transition occurs
// You can view the counts in the visualizer or access them through code
```

### Custom State Types

Create custom state types for reusable behavior:

```csharp
public abstract class AnimatedState<TContext> : StateBase<TContext> where TContext : class
{
    protected string animationTrigger;
    
    public AnimatedState(string trigger)
    {
        animationTrigger = trigger;
    }
    
    public override void Enter()
    {
        // Assuming context has an Animator property
        context.Animator.SetTrigger(animationTrigger);
    }
}
```

## Integration with Unity

### MonoBehaviour Integration

The state machine inherits from MonoBehaviour, allowing it to:
- Be attached to GameObjects
- Participate in Unity's lifecycle (Update, FixedUpdate)
- Be inspected in the editor
- Be serialized with the scene

### Editor Integration

The state machine system includes editor tools for visualization:
- State-centric view showing one state at a time
- Incoming and outgoing connections
- Real-time updates during play mode
- Navigation between states
- Transition counter display showing how many times each transition has been triggered

### Performance Considerations

The state machine system is designed to be efficient:
- States are only updated when active
- Transitions are checked in priority order
- The system stops checking transitions after the first valid one
- Lambda states avoid the overhead of creating many small classes

## Why use it?

The Cat Splat Studios State Machine provides a flexible, powerful framework for managing game states in Unity. By following the patterns and practices outlined in this document, you can create clean, maintainable state-based behavior for your games.

The system's key strengths are:
- Generic implementation that works with any context type
- Multiple ways to define states and transitions
- Fluent API for expressive, readable code
- Editor visualization for debugging
- Reactive property system for responsive behavior
- Transition counter for monitoring state flow

Whether you're creating a simple character controller or a complex game system, the state machine framework provides the tools you need to manage state effectively.
