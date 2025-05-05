# State Machine Editor Visualizer Documentation

## Overview

The State Machine Editor Visualizer is a powerful debugging and visualization tool for the Cat Splat Studios State Machine system. It provides a user-friendly interface for monitoring and interacting with state machines during development, making it easier to understand, debug, and refine state-based behavior in your Unity projects.

## Key Features

- **State-Centric Visualization**: Focuses on one state at a time, showing its connections and details
- **Real-Time Updates**: Monitors state machines during play mode with live updates
- **Interactive Navigation**: Allows navigation between connected states
- **Transition Details**: Displays conditions, actions, and priorities
- **Visual Status Indicators**: Highlights current and previous states
- **Multiple State Machine Support**: Can visualize multiple state machines in the project
- **Transition Counter**: Tracks and displays how many times each transition has been triggered

## User Interface

The visualizer window is divided into three main panels:

### 1. Left Panel (Incoming Connections)

Displays all states that can transition to the currently selected state. For each incoming connection, it shows:
- Source state name with status indicator
- Transition condition description
- Transition action (if defined)
- Transition priority (if set)
- Navigation button to go to the source state
- Part of the Connections Panel visual element class

### 2. Center Panel (Selected State)

Displays detailed information about the currently selected state:
- State name and type
- Current status (current, previous, or inactive)
- Incoming and outgoing connection counts
- State description (if available)
- Outgoing transition counts showing how many times each transition has been triggered

### 3. Right Panel (Outgoing Connections)

Displays all states that the currently selected state can transition to. For each outgoing connection, it shows:
- Target state name with status indicator
- Transition condition description
- Transition action (if defined)
- Transition priority (if set)
- Navigation button to go to the target state
- Part of the Connections Panel visual element class

### Toolbar

The toolbar at the top of the window provides:
- State machine selection dropdown
- State selection dropdown
- "Go to Current State" button
- "Go to Previous State" button
- "Live View" toggle for automatic updates

## Visual Design

The visualizer uses a custom Unity Style Sheet (USS) for consistent and attractive styling:

- **Color Coding**: Different colors indicate state status (current, previous, inactive)
- **Visual Hierarchy**: Clear visual distinction between panels and elements
- **Responsive Layout**: Adapts to different window sizes
- **Consistent Styling**: Unified look and feel across all components
- **Transition Highlighting**: Visual indication of active transitions

## How to Use the Visualizer

### Opening the Visualizer

There are two ways to open the visualizer:

1. **From the Unity Menu**:
   - Navigate to `Cat Splat State Machine > State Machine Visualizer` in the Unity menu

2. **From Code**:
   - Call `StateCentricWindow.ShowWindow()` to open the window programmatically
   - Or use the extension method: `stateMachine.OpenVisualization()`

### Registering State Machines for Visualization

Before a state machine can be visualized, it must be registered:

```csharp
// Register with automatic ID generation
stateMachine.RegisterForVisualization();

// Register with custom ID
stateMachine.RegisterForVisualization("PlayerStateMachine");

// Register and open the visualizer in one step
stateMachine.RegisterForVisualization().OpenVisualization();
```

### Navigating Between States

There are several ways to navigate between states:

1. **State Dropdown**: Select a state from the dropdown in the toolbar
2. **Navigation Buttons**: Click "Go to" buttons on connection items
3. **Current/Previous Buttons**: Use the "Go to Current State" or "Go to Previous State" buttons

### Understanding the Visualization

#### State Status

States are color-coded to indicate their status:
- **Green**: Current active state
- **Blue**: Previous state
- **Gray**: Inactive state

#### Transition Details

Transitions display several pieces of information:
- **Condition**: The condition that triggers the transition
- **Action**: Any action performed during the transition (if defined)
- **Priority**: The priority value of the transition (if not zero)

#### Active Transitions

When a transition is active (its condition is true), it is highlighted with a border. Only outgoing transitions from the current state are highlighted when active, providing clear visual indication of which transitions are currently possible.

#### Transition Counts

The visualizer displays how many times each transition has been triggered in the center panel. Outgoing transition counts are shown in the format "[SourceState] to [TargetState]: [Count]". This helps track which transitions are being used most frequently during gameplay.

## Architecture

The visualizer is built with a modular architecture:

### Core Components

1. **StateCentricWindow**: The main editor window that hosts the visualization
2. **CentralStateView**: Displays the selected state's details and transition counts
3. **ConnectionsPanel**: Shows incoming and outgoing connections with transition information

### Data Management

1. **StateMachineRegistry**: Tracks registered state machines
2. **StateMachineGraphData**: Extracts and organizes data for visualization, including transition counts

### Extension Points

1. **StateCentricVisualizationExtensions**: Provides extension methods for easy registration
2. **DescriptionAttribute**: Allows adding descriptions to states and methods

## Implementation Details

### Data Extraction

The visualizer uses reflection to extract data from state machines:
- State names and types
- Transition conditions and actions
- Current and previous state references
- Transition counter values

This allows it to work with any state machine implementation without requiring changes to the core state machine code.

### Update Cycle

The visualizer updates at regular intervals during play mode:
1. Checks for newly registered state machines
2. Updates the state machine dropdown
3. Refreshes the graph data for the selected state machine
4. Updates the visualization panels with current transition counts

### Styling

The visualizer uses Unity's UI Toolkit (formerly UIElements) for its interface:
- USS (Unity Style Sheet) defines the visual styling
- VisualElement hierarchy creates the UI structure
- Event callbacks handle user interactions

## Advanced Features

### Custom State Descriptions

You can add descriptions to states using the `DescriptionAttribute`:

```csharp
[Description("This state handles player idle behavior")]
public class PlayerIdleState : StateBase<PlayerContext>
{
    // State implementation
}
```

These descriptions will appear in the central panel of the visualizer.

### Condition Descriptions

Similarly, you can add descriptions to transition condition methods:

```csharp
[Description("Transitions when player presses the jump button")]
private bool ShouldJump()
{
    return Input.GetButtonDown("Jump");
}
```

### Multiple State Machine Visualization

The visualizer can track and display multiple state machines:
- Each state machine is identified by a unique ID
- The dropdown allows switching between registered state machines
- State machines are automatically unregistered when destroyed

## Best Practices

### Visualization Setup

1. **Register Early**: Register state machines for visualization during initialization
2. **Use Descriptive IDs**: When registering with custom IDs, use descriptive names
3. **Enable Debug Mode**: Call `stateMachine.EnableDebug()` for additional console logging

### Effective Debugging

1. **Check Current State**: Use the "Go to Current State" button to quickly see the active state
2. **Examine Transitions**: Look at incoming and outgoing connections to understand state flow
3. **Verify Conditions**: Check transition conditions to ensure they're working as expected
4. **Monitor During Play**: Keep the visualizer open during play mode to see state changes in real-time
5. **Track Transition Usage**: Use the transition counter in the center panel to see which transitions are being triggered most frequently

### UI Organization

1. **Add Descriptions**: Use the `DescriptionAttribute` to add helpful descriptions
2. **Name States Clearly**: Use descriptive names for states and transitions
3. **Prioritize Transitions**: Use priorities to make the most important transitions stand out

## Technical Limitations

1. **Reflection Usage**: The visualizer uses reflection, which may have performance implications in very complex state machines
2. **Lambda Expressions**: Condition and action descriptions may be limited for lambda expressions
3. **Play Mode Only**: Some features only work during play mode when state machines are active

## When to use it?

The State Machine Editor Visualizer is a tool for working with the Cat Splat Studios State Machine system. It provides valuable insights into state machine behavior, simplifies debugging, and helps developers create more robust state-based systems.

By using the visualizer effectively, you can:
- Better understand complex state machines
- Quickly identify and fix issues
- Refine state transitions and conditions
- Document state behavior through descriptions
- Track which transitions are being used most frequently

Whether you're developing character controllers, game mechanics, or UI systems, the visualizer makes working with state machines more intuitive and productive.
