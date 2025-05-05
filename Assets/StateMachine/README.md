# StateMachine Documentation

This documentation provides a comprehensive overview of the Cat Splat Studios StateMachine package for Unity. It includes detailed explanations of each component, usage guidelines, and best practices.

## Table of Contents

1. [Package Overview](#package-overview)
2. [Folder Structure](#folder-structure)
3. [State Machine Design](#state-machine-design)
4. [Editor Visualizer](#editor-visualizer)
5. [Detailed Component Documentation](#detailed-component-documentation)

## Package Overview

The Cat Splat Studios StateMachine is a flexible, generic implementation of the State pattern for Unity game development. It provides a robust framework for managing game states, transitions between states, and the data shared between them.

Key features:
- Generic implementation that works with any context type
- Multiple ways to define states and transitions
- Fluent API for expressive, readable code
- Editor visualization for debugging
- Reactive property system for responsive behavior

## Folder Structure

The package is organized into the following folders:

- **Core**: Contains the fundamental components of the state machine system
- **State**: Contains components related to state definitions and transitions
- **Editor**: Contains visualization and debugging tools
- **Tests**: Contains example implementations and test scripts

## State Machine Design

The state machine system is built around several key components:

1. **State Machine**: Manages states and transitions
2. **Context**: Contains shared data accessible to all states
3. **States**: Represent distinct behaviors or modes
4. **Transitions**: Define when and how states change

For a complete overview of the state machine design, see the [State Machine Design Document](StateMachine_Design_Document.md).

## Editor Visualizer

The package includes a powerful visualization tool for debugging state machines:

- State-centric visualization showing one state at a time
- Real-time updates during play mode
- Interactive navigation between states
- Detailed information about states and transitions

For a complete overview of the editor visualizer, see the [Editor Visualizer Document](Editor/Editor_Visualizer_Document.md).

## Detailed Component Documentation

For detailed documentation of each component, see the following documents:

- [Core Documentation](Core/Core_Documentation.md)
- [State Documentation](State/State_Documentation.md)
- [Editor Documentation](Editor/Editor_Documentation.md)
- [Tests Documentation](Tests/Tests_Documentation.md)
