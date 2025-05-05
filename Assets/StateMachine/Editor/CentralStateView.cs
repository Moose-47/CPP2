using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachine.Editor
{
    /// <summary>
    /// Visual component that displays detailed information about the currently selected state.
    /// </summary>
    public class CentralStateView : VisualElement
    {
        // UI elements
        private VisualElement stateContainer;
        private Label stateName;
        private Label stateType;
        private Label stateStatus;
        private VisualElement statusIndicator;
        private Label stateDescription;
        
        // Constructor
        public CentralStateView()
        {
            // Set up styles
            AddToClassList("central-state-view");
            
            // Create header
            var header = new Label("Selected State");
            header.AddToClassList("panel-header");
            Add(header);
            
            // Create state container
            stateContainer = new VisualElement();
            stateContainer.AddToClassList("state-container");
            Add(stateContainer);
            
            // Create status indicator
            statusIndicator = new VisualElement();
            statusIndicator.AddToClassList("status-indicator");
            stateContainer.Add(statusIndicator);
            
            // Create state name
            stateName = new Label("No State Selected");
            stateName.AddToClassList("state-name");
            stateContainer.Add(stateName);
            
            // Create state type
            stateType = new Label();
            stateType.AddToClassList("state-type");
            stateContainer.Add(stateType);
            
            // Create state status
            stateStatus = new Label();
            stateStatus.AddToClassList("state-status");
            stateContainer.Add(stateStatus);
            
            // Create details section
            var detailsContainer = new VisualElement();
            detailsContainer.AddToClassList("details-container");
            Add(detailsContainer);
            
            // Add details header
            var detailsHeader = new Label("State Details");
            detailsHeader.AddToClassList("details-header");
            detailsContainer.Add(detailsHeader);
            
            // Add details content
            var detailsContent = new Label("Select a state to view its details.");
            detailsContent.AddToClassList("details-content");
            detailsContainer.Add(detailsContent);
            
            // Add state description section
            var descriptionContainer = new VisualElement();
            descriptionContainer.AddToClassList("details-container");
            descriptionContainer.style.marginTop = 10;
            Add(descriptionContainer);
            
            // Add description header
            var descriptionHeader = new Label("State Description");
            descriptionHeader.AddToClassList("details-header");
            descriptionContainer.Add(descriptionHeader);
            
            // Add description content
            stateDescription = new Label("No description available.");
            stateDescription.AddToClassList("details-content");
            descriptionContainer.Add(stateDescription);
        }
        
        /// <summary>
        /// Updates the view with information about the selected state.
        /// </summary>
        /// <param name="stateNode">The state node data</param>
        /// <param name="graphData">The graph data</param>
        public void UpdateState<TContext>(StateMachineGraphData<TContext>.StateNodeData stateNode, StateMachineGraphData<TContext> graphData) where TContext : class
        {
            var detailsContent = this.Q<Label>(className: "details-content");
            if (stateNode == null)
            {
                // Clear state information
                stateName.text = "No State Selected";
                stateType.text = "";
                stateStatus.text = "";
                statusIndicator.RemoveFromClassList("current-state");
                statusIndicator.RemoveFromClassList("previous-state");
                statusIndicator.RemoveFromClassList("normal-state");
                
                // Clear details
                if (detailsContent != null)
                {
                    detailsContent.text = "Select a state to view its details.";
                }
                
                // Clear description
                stateDescription.text = "No description available.";
                return;
            }
            
            // Update state information
            stateName.text = stateNode.Name.ToUpper();

            string typeOfState;

            if (stateNode.StateType.Name.Contains("Lambda")) typeOfState = "Type: LambdaState";
            else typeOfState = $"Type: {stateNode.StateType.Name}";
            
            stateType.text = typeOfState;

            typeOfState = typeOfState.Remove(0, 6);
            
            // Update status
            if (stateNode.IsCurrentState)
            {
                stateStatus.text = "Status: Current State";
                statusIndicator.RemoveFromClassList("previous-state");
                statusIndicator.RemoveFromClassList("normal-state");
                statusIndicator.AddToClassList("current-state");
            }
            else if (stateNode.IsPreviousState)
            {
                stateStatus.text = "Status: Previous State";
                statusIndicator.RemoveFromClassList("current-state");
                statusIndicator.RemoveFromClassList("normal-state");
                statusIndicator.AddToClassList("previous-state");
            }
            else
            {
                stateStatus.text = "Status: Inactive";
                statusIndicator.RemoveFromClassList("current-state");
                statusIndicator.RemoveFromClassList("previous-state");
                statusIndicator.AddToClassList("normal-state");
            }
            
            // Count incoming and outgoing transitions
            int incomingCount = graphData.GetTransitionsToState(stateNode.Id).Count;
            int outgoingCount = graphData.GetTransitionsFromState(stateNode.Id).Count;

            // Update details
            if (detailsContent != null)
            {
                // Build the basic details text
                string detailsText = $"Incoming Connections: {incomingCount}\nOutgoing Connections: {outgoingCount}\nState Type: {typeOfState}\n";

                // Add outgoing transition counts section if there are any
                var outgoingTransitions = graphData.GetTransitionsFromState(stateNode.Id);
                if (outgoingTransitions.Count > 0)
                {
                    detailsText += "\nOutgoing Transitions Triggered:";
                    foreach (var transition in outgoingTransitions)
                    {
                        // Get target state name
                        var targetState = graphData.GetStateNodeData(transition.TargetStateId);
                        if (targetState != null)
                        {
                            detailsText += $"\n{stateNode.Name.ToUpper()} to {targetState.Name.ToUpper()}: {transition.TransitionCount}";
                        }
                    }
                }

                detailsContent.text = detailsText;
            }


            // Update description
            if (!string.IsNullOrEmpty(stateNode.StateDescription))
            {
                stateDescription.text = stateNode.StateDescription;
            }
            else
            {
                stateDescription.text = "No description available.";
            }
        }
    }
}
