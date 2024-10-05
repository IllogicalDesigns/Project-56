using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GPlanner {

    public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, object> goal, WorldStates agentProvidedStates, bool logDebug = true) {
        Dictionary<string, object> combinedStates = CreateCombinedWorldAndAgentState(agentProvidedStates);
        List<GAction> useableActions = FilterAchievableActions(actions);

        List<GNode> leaves = new List<GNode>();
        GNode start = new GNode(null, 0, combinedStates, null);

        bool success = BuildGraph(start, leaves, useableActions, goal);

        if (goal.Count == 0) {
            Debug.LogWarning("No Goals, so No plan can be found");
            return null;
        }

        if (!success) {
            Debug.LogWarning("Failed to find a successful plan");
            return null;
        }

        GNode cheapest = GetTheCheapestLeafNode(leaves);

        List<GAction> result = GetActionsToReachLeaf(cheapest);  //TODO combine these?
        Queue<GAction> queue = ConstructAQueue(result);

        DebugQueue(actions, goal, logDebug, queue);

        return queue; // Return the plan as a queue of actions
    }

    private static void DebugQueue(List<GAction> actions, Dictionary<string, object> goal, bool logDebug, Queue<GAction> queue) {
        if (logDebug) {
            string planDebug = "";
            foreach (var a in queue) {
                planDebug = planDebug + "->" + a.actionName.ToString();
            }
            var actionUserName = actions.FirstOrDefault()?.gameObject.name;
            var goalKV = goal.FirstOrDefault();
            var goalKey = goalKV.Key;
            var goalVal = goalKV.Value;
            Debug.Log("(USER:" + actionUserName + ") [GOAL:" + goalKey + "," + goalVal.ToString() + "] {PLAN:" + planDebug + "}");
        }
    }

    private static Queue<GAction> ConstructAQueue(List<GAction> result) {
        Queue<GAction> queue = new Queue<GAction>();
        foreach (var a in result) {
            queue.Enqueue(a);
        }

        return queue;
    }

    /// <summary>
    /// Creates a backwards linked list of actions from a given leaf node, starting from the root node and moving up the tree.
    ///
    /// @param leafNode The leaf node for which to create the linked list.
    /// @return A list of GAction objects representing the actions that were taken to reach the leaf node.
    /// </summary>
    private static List<GAction> GetActionsToReachLeaf(GNode leafNode) {
        // Work backwards through the cheapest node and create a linked list of nodes
        List<GAction> actionsToReachLeaf = new List<GAction>();
        GNode n = leafNode;
        while (n != null) {
            if (n.action != null) {
                actionsToReachLeaf.Insert(0, n.action);
            }
            n = n.parent;
        }

        return actionsToReachLeaf;
    }


    /// <summary>
    /// Finds the leaf node with the lowest cost from the provided list of nodes.
    ///
    /// @param leaves The list of GNode objects to search for the cheapest node.
    /// @return The GNode object with the lowest cost.
    /// </summary>
    private static GNode GetTheCheapestLeafNode(List<GNode> leaves) {

        // Find the cheapest leaf node
        GNode cheapest = null;
        foreach (var leaf in leaves) {
            if (cheapest == null)
                cheapest = leaf;
            else {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }

        return cheapest;
    }

    /// <summary>
    /// Filters a list of actions and returns only those that can be achieved.
    /// 
    /// @param actions The list of GAction objects to filter.
    /// @return A list of GAction objects that can be achieved.
    /// </summary>
    private static List<GAction> FilterAchievableActions(List<GAction> actions) {
        return actions.Where(a => a.IsAchievable()).ToList();
    }

    /// <summary>
    /// Creates a combined dictionary of world and agent state variables based on the provided WorldStates object.
    /// If the agentProvidedStates object is not null, it will be merged with the existing world states.
    ///
    /// @param agentProvidedStates The WorldStates that the agent holds onto
    /// @return A dictionary containing the combined world and agent state variables.
    /// </summary>
    private static Dictionary<string, object> CreateCombinedWorldAndAgentState(WorldStates agentProvidedStates) {
        Dictionary<string, object> combinedStates = GWorld.Instance.GetWorld().GetStates(); // Get the world states

        if (agentProvidedStates == null) return combinedStates;  //if no agent states are provided, return what we have
        
        combinedStates = combinedStates.Union(agentProvidedStates.states).ToDictionary(k => k.Key, v => v.Value); // Combine the agentProvidedStates with the world states

        return combinedStates;
    }

    /// <summary>
    /// Builds a graph of possible paths to achieve a goal by recursively exploring all possible actions.
    /// </summary>
    /// <param name="parent">The current node.</param>
    /// <param name="leaves">A list of nodes that represent achievable goals.</param>
    /// <param name="useableActions">A list of possible actions to take.</param>
    /// <param name="goal">The desired goal state.</param>
    /// <returns>true if a path to the goal has been found, false otherwise.</returns>
    private bool BuildGraph(GNode parent, List<GNode> leaves, List<GAction> useableActions, Dictionary<string, object> goal) {
        bool foundPath = false;

        // Iterate over each possible action
        foreach (var action in useableActions) {
            if (!action.IsAchievableGiven(parent.state)) continue;  // Skip unachievable actions if the parent state doesn't allow it

            var nextState = ApplyActionEffects(parent, action);
            var newNode = CreateNode(parent, parent.cost + action.GetCost(), nextState, action);

            if (GoalAchieved(goal, nextState)) { // Check if the goal is achieved
                leaves.Add(newNode); // Add the node to the list of leaves
                foundPath = true;
            }
            else {
                var subset = GetSubset(useableActions, action); // Remove the current action from the list
                foundPath |= BuildGraph(newNode, leaves, subset, goal); // Recursively build the graph
            }
        }
        return foundPath; // Return whether a path to the goal has been found

        GNode CreateNode(GNode parent, float cost, Dictionary<string, object> state, GAction action) => new GNode(parent, cost, state, action);
        List<GAction> GetSubset(List<GAction> actions, GAction excludedAction) => actions.Where(a => a != excludedAction).ToList();
    }

    private static Dictionary<string, object> ApplyActionEffects(GNode parent, GAction action) {
        Dictionary<string, object> currentState = new Dictionary<string, object>(parent.state);
        foreach (var eff in action.effects) {
            if (!currentState.ContainsKey(eff.Key)) // Apply the effects of the action to the current state
                currentState.Add(eff.Key, eff.Value);
            else  //TODO CDG check if this works?
                currentState[eff.Key] = eff.Value;
        }

        return currentState;
    }

    private bool GoalAchieved(Dictionary<string, object> goal, Dictionary<string, object> nextState) {
        foreach (var g in goal) {
            if(nextState.ContainsKey(g.Key) && nextState.ContainsKey(g.Key)) {
                return true;
            }
        }
        return false;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe) {
        List<GAction> subset = new List<GAction>();
        foreach (var a in actions) {
            if (!a.Equals(removeMe))
                subset.Add(a); // Create a subset of actions by removing the specified action
        }
        return subset; // Return the subset of actions
    }
}

public class GNode {
    public GNode parent;
    public float cost;
    public Dictionary<string, object> state;
    public GAction action;

    public GNode(GNode parent, float cost, Dictionary<string, object> allStates, GAction action) {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, object>(allStates); // Copy of allStates dictionary
        this.action = action;
    }
}
