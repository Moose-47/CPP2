# Core Documentation

The Core folder contains the fundamental components of the StateMachine system, providing the base implementation for state machines in Unity.

## Files Overview

### StateMachine.cs

This file contains the primary `StateMachine<TContext>` abstract class, which is the foundation of the entire state machine system.

**Key Features:**
- Generic implementation that works with any context type
- Inherits from MonoBehaviour for Unity integration
- Manages states and transitions between them
- Provides event notifications for state changes
- Supports debugging capabilities

**Main Components:**
- State registration and retrieval
- Transition management with condition checking
- State change functionality with enter/exit callbacks
- Support for state history (previous state tracking)
- Update and FixedUpdate lifecycle methods

**Usage Example:**
```csharp
// Create a custom state machine by inheriting from StateMachine<TContext>
public class PlayerStateMachine : StateMachine<PlayerContext> { }

// Register states and transitions
playerStateMachine.RegisterState("idle", idleState);
playerStateMachine.AddTransition(idleState, runState, () => playerContext.IsMoving);
playerStateMachine.SetInitialState("idle");
```

### ReactiveContext.cs

This file defines the `ReactiveContext<T>` abstract class, which provides a reactive property system for state machine contexts.

**Key Features:**
- Generic implementation for type-safe property change notifications
- Property change callback registration
- Helper methods for setting properties and notifying listeners

**Main Components:**
- Property change notification system
- Method chaining support for fluent API
- Type-safe implementation with generics

**Usage Example:**
```csharp
// Create a custom context by inheriting from ReactiveContext
public class PlayerContext : ReactiveContext<PlayerContext>
{
    private bool isMoving;
    
    public bool IsMoving
    {
        get => isMoving;
        set => SetProperty(ref isMoving, value, nameof(IsMoving));
    }
}

// Register for property changes
playerContext.OnPropertyChanged(nameof(PlayerContext.IsMoving), () => {
    Debug.Log("Player movement state changed!");
});
```

### ExampleContext.cs

This file provides an example implementation of a context class using the reactive context system.

**Key Features:**
- Demonstrates proper implementation of a reactive context
- Shows how to define and use reactive properties
- Includes common game-related properties for a character controller

**Main Components:**
- Movement properties (speed, jump height, etc.)
- State tracking properties (grounded, jumping, etc.)
- Combat properties (health, attacking, etc.)
- Environment interaction properties (near ladder, climbing, etc.)

### ExampleStateMachine.cs

This file contains a minimal example implementation of a state machine that uses the `ExampleContext`.

**Key Features:**
- Demonstrates how to create a concrete state machine class
- Inherits from the base `StateMachine<TContext>` class
- Uses the `ExampleContext` as its context type

**Usage:**
This class serves as a starting point for creating custom state machines. It can be extended with additional functionality specific to the game's requirements.

## Core Folder Summary

The Core folder provides the fundamental building blocks for creating state machines in Unity:

1. The `StateMachine<TContext>` class handles state management and transitions
2. The `ReactiveContext<T>` class provides a reactive property system
3. Example implementations demonstrate proper usage patterns

These components work together to create a flexible and powerful state machine system that can be adapted to various game development scenarios.
