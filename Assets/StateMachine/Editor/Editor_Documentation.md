# Editor Documentation

The Editor folder contains components for visualizing and debugging state machines in the Unity Editor. These tools provide a user-friendly interface for monitoring and interacting with state machines during development.

## Files Overview

### StateCentricWindow.cs

This file defines the `StateCentricWindow` class, which is the main editor window for visualizing state machines.

**Key Features:**
- Provides a state-centric visualization of state machines
- Shows one state at a time with incoming and outgoing connections
- Allows navigation between states
- Updates in real-time during play mode

**Main Components:**
- Editor window implementation with custom UI
- State machine and state selection dropdowns
- Navigation buttons for current and previous states
- Three-panel layout (incoming connections, selected state, outgoing connections)

**Usage Example:**
```csharp
// Open the window from code
StateCentricWindow.ShowWindow();

// Or use the menu item
// Cat Splat State Machine > State Machine Visualizer
```

### CentralStateView.cs

This file defines the `CentralStateView` class, which displays detailed information about the currently selected state.

**Key Features:**
- Shows state name, type, and status
- Displays incoming and outgoing connection counts
- Provides visual indicators for current and previous states
- Shows state description if available
- Displays transition counts for outgoing transitions

**Main Components:**
- Visual status indicator (current, previous, or inactive)
- State name and type display
- Connection count information
- State description section
- Transition counter display showing how many times each outgoing transition has been triggered

### ConnectionsPanel.cs

This file defines the `ConnectionsPanel` class, which displays connections to the selected state. Two of these panels are added. One is used for Incoming Connections and one is used for Outgoing Connections.

**Key Features:**
- Lists all states that can transition to the selected state
- Shows transition conditions, actions, and priorities
- Provides navigation to source states
- Highlights active transitions

**Main Components:**
- Scrollable list of connection items
- Visual indicators for state status
- Transition details (condition, action, priority)
- Navigation buttons

### StateMachineGraphData.cs

This file defines the `StateMachineGraphData<TContext>` class, which extracts and organizes state machine data for visualization.

**Key Features:**
- Extracts state and transition information from state machines
- Provides data structures for visualization
- Uses reflection to extract additional information
- Updates in real-time during play mode
- Tracks transition counter data for visualization

**Main Components:**
- State node data extraction
- Transition data extraction
- Methods for querying states and transitions
- Support for state descriptions and condition descriptions
- Transition counter data extraction and storage

**Inner Classes:**
- `StateNodeData` - Contains information about a state
- `TransitionData` - Contains information about a transition, including transition count
- `DescriptionAttribute` - Attribute for adding descriptions to states and methods

### StateMachineRegistry.cs

This file defines the `StateMachineRegistry` class, which tracks state machines for visualization.

**Key Features:**
- Registers state machines for visualization
- Provides access to registered state machines
- Manages graph data for visualization
- Supports non-generic access for editor tools

**Main Components:**
- Registration and unregistration methods
- Methods for accessing state machines and graph data
- Support for custom identifiers
- Non-generic access methods for editor tools

### StateCentricVisualizationExtensions.cs

This file defines extension methods for integrating the state-centric visualization with state machines.

**Key Features:**
- Provides a fluent API for registering state machines for visualization
- Simplifies the process of enabling visualization
- Enables debug mode for better visualization

**Main Components:**
- `RegisterForVisualization` extension method
- `OpenVisualization` extension method

**Usage Example:**
```csharp
// Register a state machine for visualization
stateMachine.RegisterForVisualization();

// Register with custom ID and open the visualization window
stateMachine.RegisterForVisualization("PlayerStateMachine")
           .OpenVisualization();
```

## Resources Subfolder

The Resources subfolder contains assets used by the editor visualization.

### StateCentricView.uss

This file is a Unity Style Sheet (USS) that defines the visual styling for the state-centric visualization.

**Key Features:**
- Defines colors, spacing, and layout for the visualization
- Uses CSS-like syntax for styling Unity UI elements
- Provides consistent visual styling across the visualization

**Main Components:**
- Color variables for different states and elements
- Spacing and layout definitions
- Styling for panels, state indicators, and connections
- Responsive design for different window sizes

## Editor Folder Summary

The Editor folder provides tools for visualizing and debugging state machines in the Unity Editor:

1. The `StateCentricWindow` class provides the main editor window
2. The `CentralStateView` class displays state information and transition counts for outgoing transitions
3. The `ConnectionsPanel` class displays incoming and outgoing connection information
4. The `StateMachineGraphData<TContext>` class extracts and organizes data for visualization, including transition counter data
5. The `StateMachineRegistry` class tracks state machines for visualization
6. Extension methods simplify the process of enabling visualization
7. The Resources subfolder contains styling assets

These components work together to create a powerful visualization tool that helps developers understand and debug state machines during development, with added transition counter functionality to track how many times each transition has been triggered.
