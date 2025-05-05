using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachine.Editor
{
    /// <summary>
    /// Editor window for simplified state-centric visualization of state machines.
    /// Shows one state at a time with incoming connections on the left and outgoing connections on the right.
    /// </summary>
    public class StateCentricWindow : EditorWindow
    {
        // UI elements
        private VisualElement rootElement;
        private VisualElement leftPanel;
        private VisualElement centerPanel;
        private VisualElement rightPanel;
        private DropdownField stateMachineDropdown;
        private DropdownField stateDropdown;
        private Button currentStateButton;
        private Button previousStateButton;
        private Label noStateMachineLabel;
        private Toggle liveViewToggle;
        
        // State data
        private List<string> registeredStateMachineIds = new List<string>();
        private string selectedStateMachineId;
        private string selectedStateId;
        private bool liveViewEnabled = false;
        private string currentStateId;
        
        // Components
        private CentralStateView centralStateView;
        private ConnectionsPanel incomingConnectionsPanel;
        private ConnectionsPanel outgoingConnectionsPanel;
        
        // Update timer
        private double lastUpdateTime;
        private const double UpdateInterval = 0.016; // Update every 0.5 seconds
        
        /// <summary>
        /// Shows the window.
        /// </summary>
        [MenuItem("Cat Splat State Machine/State Machine Visualizer")]
        public static void ShowWindow()
        {
            StateCentricWindow window = GetWindow<StateCentricWindow>();
            window.titleContent = new GUIContent("State Visualizer");
            window.minSize = new Vector2(800, 660);
            window.Show();
        }
        
        /// <summary>
        /// Called when the window is created.
        /// </summary>
        private void OnEnable()
        {
            // Initialize UI
            InitializeUI();
            
            // Start update loop
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        

        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        private void OnDisable()
        {
            // Stop update loop
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= PlayModeChanged;
        }

        private void PlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode)
            {
                UpdateRegisteredStateMachines();
            }
        }

        /// <summary>
        /// Initializes the UI elements.
        /// </summary>
        private void InitializeUI()
        {
            // Load and apply stylesheet
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/StateMachine/Editor/Resources/StateCentricView.uss");
            if (styleSheet != null)
            {
                rootVisualElement.styleSheets.Add(styleSheet);
            }
            
            // Create root element
            rootElement = new VisualElement();
            rootElement.AddToClassList("state-centric-view");
            rootVisualElement.Add(rootElement);
            
            // Create toolbar
            var toolbar = new Toolbar();
            toolbar.AddToClassList("toolbar");
            rootElement.Add(toolbar);
            
            // Create state machine dropdown with stacked layout
            var stateMachineContainer = new VisualElement();
            stateMachineContainer.AddToClassList("dropdown-container");
            toolbar.Add(stateMachineContainer);
            
            var stateMachineLabel = new Label("State Machine");
            stateMachineLabel.AddToClassList("dropdown-label");
            stateMachineContainer.Add(stateMachineLabel);
            
            stateMachineDropdown = new DropdownField();
            stateMachineDropdown.RegisterValueChangedCallback(OnStateMachineSelected);
            stateMachineContainer.Add(stateMachineDropdown);
            
            // Create state dropdown with stacked layout
            var stateContainer = new VisualElement();
            stateContainer.AddToClassList("dropdown-container");
            toolbar.Add(stateContainer);
            
            var stateLabel = new Label("State");
            stateLabel.AddToClassList("dropdown-label");
            stateContainer.Add(stateLabel);
            
            stateDropdown = new DropdownField();
            stateDropdown.RegisterValueChangedCallback(OnStateSelected);
            stateContainer.Add(stateDropdown);
            
            // Create current state button
            currentStateButton = new Button(GoToCurrentState);
            currentStateButton.text = "Go to Current State";
            currentStateButton.SetEnabled(false);
            toolbar.Add(currentStateButton);
            
            // Create previous state button
            previousStateButton = new Button(GoToPreviousState);
            previousStateButton.text = "Go to Previous State";
            previousStateButton.SetEnabled(false);
            toolbar.Add(previousStateButton);
            
            // Create live view toggle
            liveViewToggle = new Toggle("Live View");
            liveViewToggle.RegisterValueChangedCallback(OnLiveViewToggled);
            liveViewToggle.AddToClassList("live-view-toggle");
            toolbar.Add(liveViewToggle);
            
            // Create main container
            var mainContainer = new VisualElement();
            mainContainer.AddToClassList("main-container");
            rootElement.Add(mainContainer);
            
            // Create left panel (incoming connections)
            leftPanel = new VisualElement();
            leftPanel.AddToClassList("panel");
            leftPanel.AddToClassList("left-panel");
            mainContainer.Add(leftPanel);
            
            // Create center panel (selected state)
            centerPanel = new VisualElement();
            centerPanel.AddToClassList("panel");
            centerPanel.AddToClassList("center-panel");
            mainContainer.Add(centerPanel);
            
            // Create right panel (outgoing connections)
            rightPanel = new VisualElement();
            rightPanel.AddToClassList("panel");
            rightPanel.AddToClassList("right-panel");
            mainContainer.Add(rightPanel);
            
            // Create "no state machine" label
            noStateMachineLabel = new Label("No state machines registered for visualization.\n\nRegister a state machine using StateMachine.RegisterForVisualization() or enable DebugMode on your state machine.");
            noStateMachineLabel.AddToClassList("no-state-machine-label");
            rootElement.Add(noStateMachineLabel);
            
            // Create panel components
            centralStateView = new CentralStateView();
            incomingConnectionsPanel = new ConnectionsPanel();
            outgoingConnectionsPanel = new ConnectionsPanel(ConnectionsPanel.PanelType.Outgoing);
            
            // Set up navigation callbacks
            incomingConnectionsPanel.OnStateSelected += NavigateToState;
            outgoingConnectionsPanel.OnStateSelected += NavigateToState;
            
            // Add components to panels
            centerPanel.Add(centralStateView);
            leftPanel.Add(incomingConnectionsPanel);
            rightPanel.Add(outgoingConnectionsPanel);
            
            // Hide panels initially
            ShowPanels(false);
        }
        
        /// <summary>
        /// Called when the live view toggle is changed.
        /// </summary>
        private void OnLiveViewToggled(ChangeEvent<bool> evt)
        {
            liveViewEnabled = evt.newValue;
            
            // If live view is enabled, immediately go to current state
            if (liveViewEnabled)
            {
                GoToCurrentState();
            }

            // Enable/Disable state dropdown and current and previous buttons when in live view mode - based on the inverse of whether liveView is enabled or not.
            stateDropdown.SetEnabled(!liveViewEnabled);
            currentStateButton.SetEnabled(!liveViewEnabled);
            previousStateButton.SetEnabled(!liveViewEnabled);
            
        }
        
        /// <summary>
        /// Shows or hides the main panels.
        /// </summary>
        /// <param name="show">Whether to show the panels</param>
        private void ShowPanels(bool show)
        {
            leftPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            centerPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            rightPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            noStateMachineLabel.style.display = show ? DisplayStyle.None : DisplayStyle.Flex;
        }
        
        /// <summary>
        /// Called when the editor updates.
        /// </summary>
        private void OnEditorUpdate()
        {
            // Check if it's time to update
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastUpdateTime < UpdateInterval)
                return;
                
            lastUpdateTime = currentTime;
            
            // Update registered state machines
            UpdateRegisteredStateMachines();
            
            // If live view is enabled, check if current state has changed
            if (liveViewEnabled && !string.IsNullOrEmpty(selectedStateMachineId))
            {
                UpdateCurrentState();
            }
            // Otherwise, update visualization if a state machine and state are selected
            else if (!string.IsNullOrEmpty(selectedStateMachineId) && !string.IsNullOrEmpty(selectedStateId))
            {
                UpdateVisualization();
            }
        }
        
        /// <summary>
        /// Updates the current state and automatically navigates to it if live view is enabled.
        /// </summary>
        private void UpdateCurrentState()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId))
                return;
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state nodes
            var getAllStateNodesMethod = graphDataObj.GetType().GetMethod("GetAllStateNodes");
            if (getAllStateNodesMethod == null)
                return;
                
            var stateNodes = getAllStateNodesMethod.Invoke(graphDataObj, null) as System.Collections.IEnumerable;
            if (stateNodes == null)
                return;
                
            // Find current state
            string newCurrentStateId = null;
            foreach (var node in stateNodes)
            {
                // Use reflection to get properties
                var nodeType = node.GetType();
                var idProperty = nodeType.GetProperty("Id");
                var isCurrentStateProperty = nodeType.GetProperty("IsCurrentState");
                
                if (idProperty != null && isCurrentStateProperty != null)
                {
                    bool isCurrentState = (bool)isCurrentStateProperty.GetValue(node);
                    
                    if (isCurrentState)
                    {
                        newCurrentStateId = idProperty.GetValue(node) as string;
                        break;
                    }
                }
            }
            
            // If current state has changed, update the view
            if (newCurrentStateId != null && newCurrentStateId != currentStateId)
            {
                currentStateId = newCurrentStateId;
                selectedStateId = currentStateId;
                
                // Update dropdown selection
                UpdateStateDropdownSelection();
                
                // Update visualization
                UpdateVisualization();
            }
        }
        
        /// <summary>
        /// Updates the list of registered state machines.
        /// </summary>
        private void UpdateRegisteredStateMachines()
        {
            // Get registered state machine IDs
            registeredStateMachineIds = StateMachineRegistry.GetRegisteredStateMachineIds();
            
            // Update dropdown
            if (registeredStateMachineIds.Count > 0)
            {
                // Update state machine dropdown
                stateMachineDropdown.choices = registeredStateMachineIds;
                
                // Select first state machine if none selected
                if (string.IsNullOrEmpty(selectedStateMachineId))
                {
                    selectedStateMachineId = registeredStateMachineIds[0];
                    stateMachineDropdown.value = selectedStateMachineId;
                    UpdateStateDropdown();
                }
                
                // Show panels
                ShowPanels(true);
            }
            else
            {
                // Clear selection
                selectedStateMachineId = null;
                selectedStateId = null;
                currentStateId = null;

                // Clear dropdowns
                stateMachineDropdown.choices = new List<string>();
                stateMachineDropdown.value = string.Empty;
                stateDropdown.choices = new List<string>();
                stateDropdown.value = string.Empty;

                // Disable buttons
                currentStateButton.SetEnabled(false);
                previousStateButton.SetEnabled(false);
                liveViewToggle.SetEnabled(false);

                // Hide panels
                ShowPanels(false);
            }
        }

        /// <summary>
        /// Updates the state dropdown based on the selected state machine.
        /// </summary>
        private void UpdateStateDropdown()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId))
                return;
                
            // Get state machine using non-generic method
            var stateMachine = StateMachineRegistry.GetStateMachineObject(selectedStateMachineId);
            if (stateMachine == null)
                return;
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state nodes
            var getAllStateNodesMethod = graphDataObj.GetType().GetMethod("GetAllStateNodes");
            if (getAllStateNodesMethod == null)
                return;
                
            var stateNodes = getAllStateNodesMethod.Invoke(graphDataObj, null) as System.Collections.IEnumerable;
            if (stateNodes == null)
                return;
                
            // Create list of state names
            List<string> stateNames = new List<string>();
            Dictionary<string, string> stateNameToId = new Dictionary<string, string>();
            
            foreach (var node in stateNodes)
            {
                // Use reflection to get properties
                var nodeType = node.GetType();
                var idProperty = nodeType.GetProperty("Id");
                var nameProperty = nodeType.GetProperty("Name");
                
                if (idProperty != null && nameProperty != null)
                {
                    string id = idProperty.GetValue(node) as string;
                    string name = nameProperty.GetValue(node) as string;
                    
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                    {
                        stateNames.Add(name);
                        stateNameToId[name] = id;
                    }
                }
            }
            
            // Update state dropdown
            stateDropdown.choices = stateNames;
            
            // Select first state if none selected
            if (string.IsNullOrEmpty(selectedStateId) && stateNames.Count > 0)
            {
                string firstName = stateNames[0];
                selectedStateId = stateNameToId[firstName];
                stateDropdown.value = firstName;
            }
            else if (stateNames.Count > 0)
            {
                // Find the name for the current selected ID
                UpdateStateDropdownSelection();
            }
            
            // Update buttons
            currentStateButton.SetEnabled(true);
            previousStateButton.SetEnabled(true);
            liveViewToggle.SetEnabled(true);
        }
        
        /// <summary>
        /// Updates the state dropdown selection based on the selected state ID.
        /// </summary>
        private void UpdateStateDropdownSelection()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId) || string.IsNullOrEmpty(selectedStateId))
                return;
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state node data
            var getStateNodeDataMethod = graphDataObj.GetType().GetMethod("GetStateNodeData");
            if (getStateNodeDataMethod == null)
                return;
                
            var stateNode = getStateNodeDataMethod.Invoke(graphDataObj, new object[] { selectedStateId });
            if (stateNode == null)
                return;
                
            // Get state name
            var nodeType = stateNode.GetType();
            var nameProperty = nodeType.GetProperty("Name");
            
            if (nameProperty != null)
            {
                string name = nameProperty.GetValue(stateNode) as string;
                if (!string.IsNullOrEmpty(name))
                {
                    stateDropdown.SetValueWithoutNotify(name);
                }
            }
        }
        
        /// <summary>
        /// Updates the visualization based on the selected state machine and state.
        /// </summary>
        private void UpdateVisualization()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId) || string.IsNullOrEmpty(selectedStateId))
                return;
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state node data
            var getStateNodeDataMethod = graphDataObj.GetType().GetMethod("GetStateNodeData");
            if (getStateNodeDataMethod == null)
                return;
                
            var stateNode = getStateNodeDataMethod.Invoke(graphDataObj, new object[] { selectedStateId });
            if (stateNode == null)
                return;
                
            // Use reflection to get transitions
            var getTransitionsToStateMethod = graphDataObj.GetType().GetMethod("GetTransitionsToState");
            var getTransitionsFromStateMethod = graphDataObj.GetType().GetMethod("GetTransitionsFromState");
            
            if (getTransitionsToStateMethod == null || getTransitionsFromStateMethod == null)
                return;
                
            var incomingTransitions = getTransitionsToStateMethod.Invoke(graphDataObj, new object[] { selectedStateId });
            var outgoingTransitions = getTransitionsFromStateMethod.Invoke(graphDataObj, new object[] { selectedStateId });
            
            // Update components using reflection
            var updateStateMethod = centralStateView.GetType().GetMethod("UpdateState");
            var updateIncomingMethod = incomingConnectionsPanel.GetType().GetMethod("UpdateConnections");
            var updateOutgoingMethod = outgoingConnectionsPanel.GetType().GetMethod("UpdateConnections");
            
            if (updateStateMethod != null)
            {
                // Get the generic method with the correct type parameters
                Type graphDataType = graphDataObj.GetType();
                Type[] typeArgs = graphDataType.GetGenericArguments();
                
                if (typeArgs.Length > 0)
                {
                    // Invoke the method with the correct type parameters
                    updateStateMethod.MakeGenericMethod(typeArgs).Invoke(centralStateView, new object[] { stateNode, graphDataObj });
                    
                    if (updateIncomingMethod != null && incomingTransitions != null)
                    {
                        updateIncomingMethod.MakeGenericMethod(typeArgs).Invoke(incomingConnectionsPanel, new object[] { incomingTransitions, graphDataObj });
                    }
                    
                    if (updateOutgoingMethod != null && outgoingTransitions != null)
                    {
                        updateOutgoingMethod.MakeGenericMethod(typeArgs).Invoke(outgoingConnectionsPanel, new object[] { outgoingTransitions, graphDataObj });
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a state machine is selected from the dropdown.
        /// </summary>
        /// <param name="evt">The change event</param>
        private void OnStateMachineSelected(ChangeEvent<string> evt)
        {
            selectedStateMachineId = evt.newValue;
            selectedStateId = null;
            currentStateId = null;
            UpdateStateDropdown();
            
            // If live view is enabled, go to current state
            if (liveViewEnabled)
            {
                GoToCurrentState();
            }
            else
            {
                UpdateVisualization();
            }
        }
        
        /// <summary>
        /// Called when a state is selected from the dropdown.
        /// </summary>
        /// <param name="evt">The change event</param>
        private void OnStateSelected(ChangeEvent<string> evt)
        {
            if (liveViewEnabled)
                return; // Ignore manual selection in live view mode
                
            // Convert state name to ID
            string stateName = evt.newValue;
            
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state nodes
            var getAllStateNodesMethod = graphDataObj.GetType().GetMethod("GetAllStateNodes");
            if (getAllStateNodesMethod == null)
                return;
                
            var stateNodes = getAllStateNodesMethod.Invoke(graphDataObj, null) as System.Collections.IEnumerable;
            if (stateNodes == null)
                return;
                
            // Find state ID by name
            foreach (var node in stateNodes)
            {
                // Use reflection to get properties
                var nodeType = node.GetType();
                var idProperty = nodeType.GetProperty("Id");
                var nameProperty = nodeType.GetProperty("Name");
                
                if (idProperty != null && nameProperty != null)
                {
                    string id = idProperty.GetValue(node) as string;
                    string name = nameProperty.GetValue(node) as string;
                    
                    if (name == stateName)
                    {
                        selectedStateId = id;
                        break;
                    }
                }
            }
            
            UpdateVisualization();
        }
        
        /// <summary>
        /// Navigates to a specific state.
        /// </summary>
        /// <param name="stateId">The ID of the state to navigate to</param>
        private void NavigateToState(string stateId)
        {
            if (string.IsNullOrEmpty(stateId) || (liveViewEnabled && !currentStateButton.enabledSelf))
                return;
                
            // Disable live view if manually navigating
            if (liveViewEnabled)
            {
                liveViewToggle.value = false;
            }
            
            selectedStateId = stateId;
            UpdateStateDropdownSelection();
            UpdateVisualization();
        }
        
        /// <summary>
        /// Navigates to the current state of the selected state machine.
        /// </summary>
        private void GoToCurrentState()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId))
                return;
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state nodes
            var getAllStateNodesMethod = graphDataObj.GetType().GetMethod("GetAllStateNodes");
            if (getAllStateNodesMethod == null)
                return;
                
            var stateNodes = getAllStateNodesMethod.Invoke(graphDataObj, null) as System.Collections.IEnumerable;
            if (stateNodes == null)
                return;
                
            // Find current state
            foreach (var node in stateNodes)
            {
                // Use reflection to get properties
                var nodeType = node.GetType();
                var idProperty = nodeType.GetProperty("Id");
                var isCurrentStateProperty = nodeType.GetProperty("IsCurrentState");
                
                if (idProperty != null && isCurrentStateProperty != null)
                {
                    bool isCurrentState = (bool)isCurrentStateProperty.GetValue(node);
                    
                    if (isCurrentState)
                    {
                        string id = idProperty.GetValue(node) as string;
                        currentStateId = id;
                        selectedStateId = id;
                        UpdateStateDropdownSelection();
                        UpdateVisualization();
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// Navigates to the previous state of the selected state machine.
        /// </summary>
        private void GoToPreviousState()
        {
            if (string.IsNullOrEmpty(selectedStateMachineId))
                return;
                
            // Disable live view if manually navigating
            //if (liveViewEnabled)
            //{
            //    liveViewToggle.value = false;
            //}
                
            // Get graph data using non-generic method
            var graphDataObj = StateMachineRegistry.GetGraphDataObject(selectedStateMachineId);
            if (graphDataObj == null)
                return;
                
            // Use reflection to get state nodes
            var getAllStateNodesMethod = graphDataObj.GetType().GetMethod("GetAllStateNodes");
            if (getAllStateNodesMethod == null)
                return;
                
            var stateNodes = getAllStateNodesMethod.Invoke(graphDataObj, null) as System.Collections.IEnumerable;
            if (stateNodes == null)
                return;
                
            // Find previous state
            foreach (var node in stateNodes)
            {
                // Use reflection to get properties
                var nodeType = node.GetType();
                var idProperty = nodeType.GetProperty("Id");
                var isPreviousStateProperty = nodeType.GetProperty("IsPreviousState");
                
                if (idProperty != null && isPreviousStateProperty != null)
                {
                    bool isPreviousState = (bool)isPreviousStateProperty.GetValue(node);
                    
                    if (isPreviousState)
                    {
                        string id = idProperty.GetValue(node) as string;
                        selectedStateId = id;
                        UpdateStateDropdownSelection();
                        UpdateVisualization();
                        return;
                    }
                }
            }
        }
    }
}
