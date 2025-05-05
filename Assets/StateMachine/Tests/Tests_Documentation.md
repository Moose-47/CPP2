# Tests Folder Documentation

The Tests folder contains test scripts that demonstrate and validate the functionality of the StateMachine system. These tests serve as both validation tools and examples of how to use the system.

## Files Overview

### StateMachineTest.cs

This file defines the `StateMachineTest` class, which is a MonoBehaviour that sets up and tests a simple state machine during runtime.

**Key Features:**
- Demonstrates practical usage of the state machine system
- Creates and configures a state machine with multiple states
- Shows how to use the builder pattern for states and transitions
- Provides runtime testing of state transitions

**Main Components:**
- Setup of an `ExampleStateMachine` with an `ExampleContext`
- Creation of test states (idle, move, jump) using the `StateBuilder`
- Configuration of transitions between states
- Input handling for testing transitions
- Debug logging for monitoring state changes

**Test Workflow:**
1. Creates a player context with references to components
2. Adds a state machine component to the GameObject
3. Creates test states using the `StateBuilder`
4. Registers states with string identifiers
5. Sets up transitions between states with conditions and actions
6. Sets the initial state
7. Enables debug messaging
8. Updates input in the context during gameplay
9. Provides debug output when Tab key is pressed

**Usage Example:**
```csharp
// Attach this script to a GameObject to test the state machine
// Press WASD to move, Space to jump, Tab to check current state
```

## Tests Folder Summary

The Tests folder provides practical examples and validation tools for the StateMachine system:

1. The `StateMachineTest` class demonstrates how to set up and use a state machine
2. The test script shows the complete workflow from creation to runtime usage
3. Debug logging helps verify that the system is working correctly
4. Input handling demonstrates how to trigger state transitions

These test components serve as both validation tools and learning resources for developers using the StateMachine system. By examining the test code, developers can understand how to implement state machines in their own projects.
